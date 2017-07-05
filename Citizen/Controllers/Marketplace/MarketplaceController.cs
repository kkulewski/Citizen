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

namespace Citizen.Controllers.Marketplace
{
    [Authorize]
    public class MarketplaceController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public MarketplaceController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Marketplace
        public async Task<IActionResult> Index(StatusMessageId? message = null)
        {
            ViewData["StatusMessage"] =
                  message == StatusMessageId.AddOfferSuccess ? "Offer added succesfully."
                : message == StatusMessageId.EditOfferSuccess ? "Offer updated succesfully."
                : "";


            var user = await _userManager.GetUserAsync(HttpContext.User);

            var offers = _context.MarketplaceOffers
                .Where(offer => offer.ApplicationUser.Id == user.Id)
                .OrderBy(offer => offer.Price)
                .Include(m => m.ApplicationUser);
            
            return View("~/Views/Marketplace/Index.cshtml", await offers.ToListAsync());
        }

        // GET: Marketplace
        public async Task<IActionResult> Offers(ItemType id)
        {
            var user = _userManager.GetUserAsync(HttpContext.User);
            ViewData["ApplicationUser"] = user;

            var offers = _context.MarketplaceOffers
                .Where(offer => offer.ItemType == id)
                .OrderBy(offer => offer.Price)
                .Include(m => m.ApplicationUser);

            return View("~/Views/Marketplace/Offers.cshtml", await offers.ToListAsync());
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
                message == StatusMessageId.AddOfferSuccess ? "Offer added succesfully."
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

            return View(marketplaceOffer);
        }

        // POST: Marketplace/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var marketplaceOffer = await _context.MarketplaceOffers.SingleOrDefaultAsync(m => m.Id == id);
            _context.MarketplaceOffers.Remove(marketplaceOffer);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool MarketplaceOfferExists(int id)
        {
            return _context.MarketplaceOffers.Any(e => e.Id == id);
        }
        
        public enum StatusMessageId
        {
            AddOfferSuccess,
            AddOfferPriceInvalid,
            AddOfferAmountInvalid,
            AddOfferAmountNotAvailable,
            EditOfferSuccess,
            EditOfferPriceInvalid,
            Error
        }
    }
}
