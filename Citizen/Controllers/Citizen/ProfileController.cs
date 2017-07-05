using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Citizen.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Citizen.Models;
using Citizen.Models.CitizenViewModels;
using Citizen.Services;

namespace Citizen.Controllers.Citizen
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger _logger;
        private readonly ApplicationDbContext _dbContext;

        public ProfileController(
          UserManager<ApplicationUser> userManager,
          ILoggerFactory loggerFactory,
          ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _logger = loggerFactory.CreateLogger<ProfileController>();
            _dbContext = dbContext;
        }

        //
        // GET: /Manage/Index
        [HttpGet]
        public async Task<IActionResult> Index(StatusMessageId? message = null)
        {
            ViewData["StatusMessage"] =
                message == StatusMessageId.ChangeCountrySuccess ? "Your country has been changed."
                : message == StatusMessageId.ChangeCountryNotEnoughMoney ? "Country change not possible - not enough money."
                : message == StatusMessageId.EatNoFoodAvailable ? "No food available."
                : message == StatusMessageId.EatEnergyMax ? "Energy max."
                : message == StatusMessageId.EatNoEnergyRestoreAvailable ? "No energy restore available."
                : message == StatusMessageId.EatSuccess ? "Food eaten."
                : message == StatusMessageId.Error ? "An error has occurred."
                : "";

            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return View("Error");
            }
            
            var userCountry = _dbContext.Country.First(country => country.Id == user.CountryId);

            var model = new ProfileViewModel
            {
                Name = user.Name,
                Energy = user.Energy,
                EnergyMax = GameSettings.EnergyMax,
                EnergyRestore = user.EnergyRestore,
                Money = user.Money,
                Country = userCountry
            };
            return View("~/Views/Citizen/Profile/Index.cshtml", model);
        }

        [HttpGet]
        public async Task<IActionResult> SubstractEnergyTest(int amount)
        {
            var user = await GetCurrentUserAsync();
            user.Energy -= 20;
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { Message = StatusMessageId.Error });
        }

        [HttpGet]
        public async Task<IActionResult> Eat()
        {
            var user = await GetCurrentUserAsync();
            var userItems = _dbContext.Items.Where(u => u.ApplicationUserId == user.Id);
            var foodItem = userItems.First(c => c.ItemType == ItemType.Food);

            if (foodItem.Amount == 0)
            {
                return RedirectToAction(nameof(Index), new { Message = StatusMessageId.EatNoFoodAvailable });
            }

            if (user.EnergyRestore == 0)
            {
                return RedirectToAction(nameof(Index), new { Message = StatusMessageId.EatNoEnergyRestoreAvailable });
            }

            if (user.Energy == GameSettings.EnergyMax)
            {
                return RedirectToAction(nameof(Index), new { Message = StatusMessageId.EatEnergyMax });
            }

            var food = new ConsumableItem
            {
                EnergyRestoreAmount = GameSettings.FoodEnergyRestore,
                Amount = foodItem.Amount
            };

            while (user.Eat(food)) { }

            foodItem.Amount = food.Amount;
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { Message = StatusMessageId.EatSuccess });
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

            var countryList = _dbContext.Country.ToList();
            var userCountry = _dbContext.Country.First(country => country.Id == user.CountryId);

            var model = new ChangeCountryViewModel()
            {
                Money = user.Money,
                CountryId = userCountry.Id,
                CountryList = countryList,
                Country = user.Country,
                CountryChangeCost = GameSettings.CountryChangeCost
        };

            return View("~/Views/Citizen/Profile/ChangeCountry.cshtml", model);
        }

        //
        // POST: /Manage/ChangeCountry
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeCountry(ChangeCountryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Citizen/Profile/ChangeCountry.cshtml", model);
            }
            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                user.CountryId = model.CountryId;

                if (user.Money < model.CountryChangeCost)
                {
                    return RedirectToAction(nameof(Index), new { Message = StatusMessageId.ChangeCountryNotEnoughMoney });
                }

                user.Money -= model.CountryChangeCost;
                _dbContext.SaveChanges();

                return RedirectToAction(nameof(Index), new { Message = StatusMessageId.ChangeCountrySuccess });
            }
            return RedirectToAction(nameof(Index), new { Message = StatusMessageId.Error });
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

            var userCountry = _dbContext.Country.First(country => country.Id == user.CountryId);
            var userStorage = _dbContext.UserStorage.First(storage => storage.ApplicationUserId == user.Id);
            var userItems = _dbContext.Items.Where(it => it.ApplicationUserId == user.Id);

            int capacityUsed = 0;
            foreach (var item in userItems)
            {
                capacityUsed += item.Amount;
            }

            var model = new UserStorageViewModel
            {
                Name = user.Name,
                Energy = user.Energy,
                Money = user.Money,
                Country = userCountry,
                Capacity = userStorage.Capacity,
                CapacityUsed =  capacityUsed,
                MarketPlaceholder = userItems.First(c => c.ItemType == ItemType.MarketPlaceholder).Amount,
                Food = userItems.First(c => c.ItemType == ItemType.Food).Amount,
                Grain = userItems.First(c => c.ItemType == ItemType.Grain).Amount
            };
            return View("~/Views/Citizen/Profile/UserStorage.cshtml", model);
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        public enum StatusMessageId
        {
            ChangeCountrySuccess,
            ChangeCountryNotEnoughMoney,
            EatNoFoodAvailable,
            EatEnergyMax,
            EatNoEnergyRestoreAvailable,
            EatSuccess,
            Error
        }

        private Task<ApplicationUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }

        #endregion
    }
}
