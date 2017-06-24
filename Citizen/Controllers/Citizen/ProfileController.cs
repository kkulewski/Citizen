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
using Citizen.Models.ManageViewModels;
using Citizen.Services;
using Citizen.Models.Items;

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
        public async Task<IActionResult> Index(ManageMessageId? message = null)
        {
            ViewData["StatusMessage"] =
                message == ManageMessageId.ChangeCountrySuccess ? "Your country has been changed."
                : message == ManageMessageId.ChangeCountryNotEnoughMoney ? "Country change not possible - not enough money."
                : message == ManageMessageId.EatFoodSuccess ? "Food eaten."
                : message == ManageMessageId.EatFoodFailure ? "No food available."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";

            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return View("Error");
            }

            var countries = from c in _dbContext.Country select c;
            var foodItems = from c in _dbContext.FoodItem select c;
            var userCountry = countries.First(country => country.Id == user.CountryId);
            var userFoodItem = foodItems.First(foodItem => foodItem.ApplicationUserId == user.Id);

            var model = new IndexViewModel
            {
                Name = user.Name,
                Energy = user.Energy,
                EnergyRestore = user.EnergyRestore,
                Money = user.Money,
                Country = userCountry,
                FoodItem = userFoodItem
            };
            return View("~/Views/Citizen/Profile/Index.cshtml", model);
        }

        [HttpGet]
        public async Task<IActionResult> ConsumeTest()
        {
            var user = await GetCurrentUserAsync();

            var food = new FoodItem();
            food.Amount = 5;
            food.EnergyRecoverAmount = 300;
            food.Name = "Food";

            user.Consume(food);

            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { Message = ManageMessageId.Error });
        }

        [HttpGet]
        public async Task<IActionResult> AddFoodItemTest()
        {
            var user = await GetCurrentUserAsync();
            
            var foodItems = from c in _dbContext.FoodItem select c;
            var userFoodItem = foodItems.FirstOrDefault(foodItem => foodItem.ApplicationUserId == user.Id);
            user.FoodItem = userFoodItem;
            
            if(user.FoodItem == null)
            {
                var foodItem = new FoodItem();
                foodItem.Name = "Food";
                foodItem.Amount = 5;
                foodItem.EnergyRecoverAmount = 10;

                user.FoodItem = foodItem;
            }
            else
            {
                user.FoodItem.Amount += 5;
            }
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { Message = ManageMessageId.Error });
        }

        [HttpGet]
        public async Task<IActionResult> EatFood()
        {
            var user = await GetCurrentUserAsync();
            var foodItems = from c in _dbContext.FoodItem select c;
            var userFoodItem = foodItems.FirstOrDefault(foodItem => foodItem.ApplicationUserId == user.Id);
            user.FoodItem = userFoodItem;

            if (user.FoodItem.Amount > 0)
            {
                user.Consume(user.FoodItem);
                await _dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { Message = ManageMessageId.EatFoodSuccess });
            }

            return RedirectToAction(nameof(Index), new { Message = ManageMessageId.EatFoodFailure });
        }

        [HttpGet]
        public async Task<IActionResult> SubstractEnergyTest()
        {
            var user = await GetCurrentUserAsync();
            user.Energy -= 200;
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { Message = ManageMessageId.Error });
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

            var countries = from c in _dbContext.Country select c;
            var userCountry = countries.First(country => country.Id == user.CountryId);

            decimal countryChangeCost = 5.00M;

            var model = new ChangeCountryViewModel()
            {
                Money = user.Money,
                CountryId = userCountry.Id,
                CountryList = countryList,
                Country = user.Country,
                CountryChangeCost = countryChangeCost
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
                return View(model);
            }
            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                user.CountryId = model.CountryId;

                if (user.Money < model.CountryChangeCost)
                {
                    return RedirectToAction(nameof(Index), new { Message = ManageMessageId.ChangeCountryNotEnoughMoney });
                }

                user.Money -= model.CountryChangeCost;
                _dbContext.SaveChanges();

                return RedirectToAction(nameof(Index), new { Message = ManageMessageId.ChangeCountrySuccess });
            }
            return RedirectToAction(nameof(Index), new { Message = ManageMessageId.Error });
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        public enum ManageMessageId
        {
            ChangeCountrySuccess,
            ChangeCountryNotEnoughMoney,
            EatFoodSuccess,
            EatFoodFailure,
            Error
        }

        private Task<ApplicationUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }

        #endregion
    }
}
