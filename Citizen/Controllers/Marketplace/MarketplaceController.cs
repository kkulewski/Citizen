using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _dbContext;

        public MarketplaceController(UserManager<ApplicationUser> userManager, ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }

        // GET: Marketplace
        public async Task<IActionResult> Index(string message)
        {
            if (message != null)
            {
                ViewData["StatusMessage"] = message;
            }

            var user = await GetCurrentUserAsync();
            return View(user.MarketplaceOffers.OrderBy(i => i.Price));
        }

        // GET: Marketplace/Offers/Food
        public async Task<IActionResult> Offers(string message, ItemType item)
        {
            if (message != null)
            {
                ViewData["StatusMessage"] = message;
            }

            var user = await GetCurrentUserAsync();
            ViewData["ApplicationUser"] = user;
            return View(GetOffersByItemType(item).OrderBy(i => i.Price));
        }

        // GET: Marketplace/Add
        public IActionResult Add(string message)
        {
            if (message != null)
            {
                ViewData["StatusMessage"] = message;
            }

            var itemTypes = new List<ItemType>()
            {
                ItemType.Food,
                ItemType.Grain
            };

            var viewModel = new AddMarketplaceOfferViewModel() { ItemTypes = itemTypes };
            return View(viewModel);
        }
        
        // POST: /Marketplace/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddMarketplaceOfferViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await GetCurrentUserAsync();
            var result = user.AddMarketplaceOffer(model.ItemType, model.Amount, model.Price);
            if(result.Success)
            {
                await SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { result.Message });
            }
            
            return RedirectToAction(nameof(Add), new { result.Message });
        }

        // GET: Marketplace/Edit/5
        public async Task<IActionResult> Edit(int? id, string message)
        {
            if (message != null)
            {
                ViewData["StatusMessage"] = message;
            }

            if (id == null)
            {
                return NotFound();
            }
            
            var offer = await GetOfferByIdAsync(id.Value);
            if (offer == null)
            {
                return NotFound();
            }

            var viewModel = new EditMarketplaceOfferViewModel()
            {
                Id = offer.Id,
                ItemType = offer.ItemType,
                Amount = offer.Amount,
                Price = offer.Price
            };

            return View(viewModel);
        }
        
        // POST: /Marketplace/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditMarketplaceOfferViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            
            var user = await GetCurrentUserAsync();
            var result = user.EditMarketplaceOffer(model.Id, model.Price);
            if(result.Success)
            {
                await SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { result.Message });
            }

            return RedirectToAction(nameof(Edit), new { result.Message });
        }

        // GET: Marketplace/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var offer = await GetOfferByIdAsync(id.Value);
            if (offer == null)
            {
                return NotFound();
            }

            var viewModel = new DeleteMarketplaceOfferViewModel()
            {
                Id = offer.Id,
                ItemType = offer.ItemType,
                Amount = offer.Amount,
                Price = offer.Price
            };

            return View(viewModel);
        }

        // POST: Marketplace/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(DeleteMarketplaceOfferViewModel model)
        {
            var user = await GetCurrentUserAsync();
            var result = user.DeleteMarketplaceOffer(model.Id);
            if(result.Success)
            {
                await SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index), new { result.Message });
        }

        [HttpPost]
        public async Task<IActionResult> Buy(int id, int buyAmount)
        {
            var user = await GetCurrentUserAsync();
            var offer = await GetOfferWithSellerByIdAsync(id);

            var result = user.BuyMarketplaceOffer(offer, buyAmount);
            if(result.Success)
            {
                DeleteOfferIfAmountIsZero(offer);
                await SaveChangesAsync();
            }

            return RedirectToAction(nameof(Offers), new { item = offer.ItemType, message = result.Message });
        }

        #region Helpers

        private async Task<ApplicationUser> GetCurrentUserAsync()
        {
            var identityUser = await _userManager.GetUserAsync(HttpContext.User);
            return await _dbContext.ApplicationUsers
                .Include(p => p.Items)
                .Include(p => p.MarketplaceOffers)
                .Include(p => p.Country)
                .Include(p => p.UserStorage)
                .FirstOrDefaultAsync(u => u.Id == identityUser.Id);
        }
        
        public IQueryable<MarketplaceOffer> GetOffersByItemType(ItemType itemType)
        {
            return _dbContext.MarketplaceOffers
                .Include(m => m.ApplicationUser)
                .Where(m => m.ItemType == itemType);
        }
        
        public Task<MarketplaceOffer> GetOfferByIdAsync(int id)
        {
            return _dbContext.MarketplaceOffers
                .Include(m => m.ApplicationUser)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public Task<MarketplaceOffer> GetOfferWithSellerByIdAsync(int id)
        {
            return _dbContext.MarketplaceOffers
                .Include(m => m.ApplicationUser)
                .Include(m => m.ApplicationUser.Items)
                .Include(m => m.ApplicationUser.UserStorage)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public void DeleteOfferIfAmountIsZero(MarketplaceOffer offer)
        {
            if (offer.Amount == 0)
            {
                _dbContext.MarketplaceOffers.Remove(offer);
            }
        }

        private async Task<ActionStatus> SaveChangesAsync()
        {
            try
            {
                await _dbContext.SaveChangesAsync();
                return new ActionStatus(true, GameSettings.DataConcurrencyOk);
            }
            catch
            {
                return new ActionStatus(false, GameSettings.DataConcurrencyError);
            }
        }

        #endregion
    }
}
