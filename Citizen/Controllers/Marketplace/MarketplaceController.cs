using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Citizen.Data;
using Citizen.Models;
using Citizen.Models.MarketplaceViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Citizen.DAL;

namespace Citizen.Controllers.Marketplace
{
    [Authorize]
    public class MarketplaceController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository _repo;

        public MarketplaceController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IRepository repository)
        {
            _context = context;
            _userManager = userManager;
            _repo = repository;
        }

        // GET: Marketplace
        public async Task<IActionResult> Index(string message)
        {
            if (message != null)
            {
                ViewData["StatusMessage"] = message;
            }

            var user = await GetCurrentUserAsync();
            
            return View("~/Views/Marketplace/Index.cshtml", user.MarketplaceOffers);
        }

        // GET: Marketplace
        public async Task<IActionResult> Offers(ItemType itemType)
        {
            var user = await GetCurrentUserAsync();
            ViewData["ApplicationUser"] = user;

            return View("~/Views/Marketplace/Offers.cshtml", _repo.MarketplaceService.GetOffers(itemType));
        }

        // GET: Marketplace/AddOffer
        public IActionResult AddOffer(StatusMessageId? message = null)
        {
            ViewData["StatusMessage"] =
                  message == StatusMessageId.AddOfferAmountInvalid ? "Error - invalid amount."
                : message == StatusMessageId.AddOfferPriceInvalid ? "Error - invalid price."
                : message == StatusMessageId.AddOfferAmountNotAvailable ? "Error - amount not available."
                : message == StatusMessageId.Error ? "Error."
                : "";

            IEnumerable<ItemType> itemTypes = new List<ItemType>()
            {
                ItemType.Food,
                ItemType.Grain
            };

            var addMarketplaceOfferViewModel = new AddMarketplaceOfferViewModel()
            {
                ItemTypes = itemTypes
            };

            return View(addMarketplaceOfferViewModel);
        }

        //
        // POST: /Marketplace/AddOffer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOffer(AddMarketplaceOfferViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(AddOffer), new { Message = StatusMessageId.Error });
            }

            var user = await _userManager.GetUserAsync(HttpContext.User);

            var userItems = _context.Items.Where(it => it.ApplicationUserId == user.Id);
            var userItem = userItems.First(i => i.ItemType == model.ItemType);
            var userMarketPlaceholder = userItems.First(i => i.ItemType == ItemType.MarketPlaceholder);

            // amount invalid
            if (model.Amount <= 0)
            {
                return RedirectToAction(nameof(AddOffer), new { Message = StatusMessageId.AddOfferAmountInvalid });
            }

            // amount not available
            if (model.Amount > userItem.Amount)
            {
                return RedirectToAction(nameof(AddOffer), new { Message = StatusMessageId.AddOfferAmountNotAvailable });
            }

            // price is invalid
            if (model.Price <= 0.00M)
            {
                return RedirectToAction(nameof(AddOffer), new { Message = StatusMessageId.AddOfferPriceInvalid });
            }

            // remove specified amount from user, add to market offer
            // TODO: add storage placeholder to avoid item overstorage in market
            userItem.Amount -= model.Amount;
            userMarketPlaceholder.Amount += model.Amount;

            var offer = new MarketplaceOffer
            {
                ApplicationUserId = user.Id,
                ItemType = model.ItemType,
                Amount = model.Amount,
                Price = model.Price
            };
            user.MarketplaceOffers.Add(offer);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { Message = StatusMessageId.AddOfferSuccess });
        }

        // GET: Marketplace/EditOffer/5
        public async Task<IActionResult> EditOffer(int? id, StatusMessageId? message = null)
        {
            ViewData["StatusMessage"] =
                      message == StatusMessageId.EditOfferPriceInvalid ? "Error - price invalid."
                    : message == StatusMessageId.Error ? "Error."
                    : "";

            if (id == null)
            {
                return NotFound();
            }

            var marketplaceOffer = await _context.MarketplaceOffers.SingleOrDefaultAsync(m => m.Id == id);
            if (marketplaceOffer == null)
            {
                return NotFound();
            }

            var editMarketplaceOfferViewModel = new EditMarketplaceOfferViewModel()
            {
                Id = marketplaceOffer.Id,
                ItemType = marketplaceOffer.ItemType,
                Amount = marketplaceOffer.Amount,
                Price = marketplaceOffer.Price
            };

            return View(editMarketplaceOfferViewModel);
        }

        //
        // POST: /Marketplace/EditOffer/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditOffer(EditMarketplaceOfferViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(EditOffer), new { Message = StatusMessageId.Error });
            }

            if (model.Price <= 0.00M)
            {
                return RedirectToAction(nameof(EditOffer), new { Message = StatusMessageId.EditOfferPriceInvalid });
            }

            var marketplaceOffer = await _context.MarketplaceOffers.SingleOrDefaultAsync(m => m.Id == model.Id);
            if (marketplaceOffer == null)
            {
                return NotFound();
            }

            var newPrice = model.Price;
            marketplaceOffer.Price = newPrice;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { Message = StatusMessageId.EditOfferSuccess });
        }

        // GET: Marketplace/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var marketplaceOffer = await _context.MarketplaceOffers
                .Include(m => m.ApplicationUser)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (marketplaceOffer == null)
            {
                return NotFound();
            }

            var deleteMarketplaceOfferViewModel = new DeleteMarketplaceOfferViewModel()
            {
                Id = marketplaceOffer.Id,
                ItemType = marketplaceOffer.ItemType,
                Amount = marketplaceOffer.Amount,
                Price = marketplaceOffer.Price
            };

            return View(deleteMarketplaceOfferViewModel);
        }

        // POST: Marketplace/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(DeleteMarketplaceOfferViewModel model)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            var userItems = _context.Items.Where(it => it.ApplicationUserId == user.Id);
            var userItem = userItems.First(i => i.ItemType == model.ItemType);
            var userMarketPlaceholder = userItems.First(i => i.ItemType == ItemType.MarketPlaceholder);

            var marketplaceOffer = await _context.MarketplaceOffers.SingleOrDefaultAsync(m => m.Id == model.Id);

            userItem.Amount += marketplaceOffer.Amount;
            userMarketPlaceholder.Amount -= marketplaceOffer.Amount;

            _context.MarketplaceOffers.Remove(marketplaceOffer);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { Message = StatusMessageId.DeleteOfferSuccess });
        }
        
        public enum StatusMessageId
        {
            AddOfferSuccess,
            AddOfferPriceInvalid,
            AddOfferAmountInvalid,
            AddOfferAmountNotAvailable,
            EditOfferSuccess,
            EditOfferPriceInvalid,
            DeleteOfferSuccess,
            Error
        }

        private async Task<ApplicationUser> GetCurrentUserAsync()
        {
            var identityUser = await _userManager.GetUserAsync(HttpContext.User);
            return _repo.ApplicationUserService.GetApplicationUserById(identityUser.Id);
        }
    }
}
