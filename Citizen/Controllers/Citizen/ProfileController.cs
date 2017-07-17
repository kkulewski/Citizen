using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Citizen.Models;
using Citizen.Models.CitizenViewModels;
using Citizen.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Citizen.Controllers.Citizen
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _dbContext;

        public ProfileController(
          UserManager<ApplicationUser> userManager,
          ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }

        //
        // GET: /Manage/Index
        [HttpGet]
        public async Task<IActionResult> Index(string message)
        {
            if (message != null)
            {
                ViewData["StatusMessage"] = message;
            }

            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return View("Error");
            }

            var model = new ProfileViewModel
            {
                Name = user.Name,
                Energy = user.Energy,
                EnergyMax = GameSettings.EnergyMax,
                EnergyRestore = user.EnergyRestore,
                Money = user.Money,
                Country = user.Country
            };
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Eat()
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return View("Error");
            }

            var result = user.Eat();
            if (result.Success)
            {
                await SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index), new { result.Message });
        }

        //
        // GET: /Manage/ChangeCountry
        [HttpGet]
        public async Task<IActionResult> ChangeCountry()
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return View("Error");
            }

            var model = new ChangeCountryViewModel()
            {
                Money = user.Money,
                CountryId = user.Country.Id,
                CountryList = await GetCountryListAsync(),
                Country = user.Country,
                CountryChangeCost = GameSettings.CountryChangeCost
            };

            return View(model);
        }

        //
        // POST: /Manage/ChangeCountry
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeCountry(ChangeCountryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return View("Error");
            }

            var newCountry = await GetCountryByIdAsync(model.CountryId);
            var result = user.ChangeCountry(newCountry);
            if(result.Success)
            {
                await SaveChangesAsync();
            }
            
            return RedirectToAction(nameof(Index), new { result.Message });
        }

        //
        // GET: /Profile/Storage
        [HttpGet]
        public async Task<IActionResult> Storage()
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return View("Error");
            }

            var model = new UserStorageViewModel
            {
                Name = user.Name,
                Energy = user.Energy,
                Money = user.Money,
                Country = user.Country,
                Capacity = user.UserStorage.Capacity,
                CapacityUsed =  user.GetItemsAmount(),
                ExtensionCost = user.GetStorageExtensionCost(),
                MarketPlaceholder = user.Items.First(c => c.ItemType == ItemType.MarketPlaceholder).Amount,
                Food = user.Items.First(c => c.ItemType == ItemType.Food).Amount,
                Grain = user.Items.First(c => c.ItemType == ItemType.Grain).Amount
            };
            return View(model);
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

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

        private async Task<Country> GetCountryByIdAsync(int id)
        {
            return await _dbContext.Country.FirstOrDefaultAsync(c => c.Id == id);
        }

        private async Task<List<Country>> GetCountryListAsync()
        {
            return await _dbContext.Country.ToListAsync();
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
