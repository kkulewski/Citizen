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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository _repo;

        private const string _modelInvalidMessage = "Error - model invalid.";

        public MarketplaceController(UserManager<ApplicationUser> userManager, IRepository repository)
        {
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
        public IActionResult AddOffer(string message)
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

        //
        // POST: /Marketplace/AddOffer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOffer(AddMarketplaceOfferViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(AddOffer), _modelInvalidMessage);
            }

            var user = await GetCurrentUserAsync();
            var result = user.AddMarketplaceOffer(model.ItemType, model.Amount, model.Price);
            if(result.Success)
            {
                await _repo.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { result.Message });
            }
            
            return RedirectToAction(nameof(AddOffer), new { result.Message });
        }

        // GET: Marketplace/EditOffer/5
        public async Task<IActionResult> EditOffer(int? id, string message)
        {
            if (message != null)
            {
                ViewData["StatusMessage"] = message;
            }

            if (id == null)
            {
                return NotFound();
            }
            
            var offer = await _repo.MarketplaceService.GetOfferById(id.Value);
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

        //
        // POST: /Marketplace/EditOffer/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditOffer(EditMarketplaceOfferViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(EditOffer), _modelInvalidMessage);
            }
            
            var user = await GetCurrentUserAsync();
            var result = user.EditMarketplaceOffer(model.Id, model.Price);
            if(result.Success)
            {
                await _repo.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { result.Message });
            }

            return RedirectToAction(nameof(EditOffer), new { result.Message });
        }

        // GET: Marketplace/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var offer = await _repo.MarketplaceService.GetOfferById(id.Value);
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
                await _repo.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index), new { result.Message });
        }

        private async Task<ApplicationUser> GetCurrentUserAsync()
        {
            var identityUser = await _userManager.GetUserAsync(HttpContext.User);
            return _repo.ApplicationUserService.GetApplicationUserById(identityUser.Id);
        }
    }
}
