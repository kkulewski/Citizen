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
using Citizen.DAL;

namespace Citizen.Controllers.Citizen
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly IRepository _repo;

        public ProfileController(
          UserManager<ApplicationUser> userManager,
          ILoggerFactory loggerFactory,
          ApplicationDbContext dbContext,
          IRepository repository)
        {
            _userManager = userManager;
            _logger = loggerFactory.CreateLogger<ProfileController>();
            _dbContext = dbContext;
            _repo = repository;
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
            return View("~/Views/Citizen/Profile/Index.cshtml", model);
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
                await _repo.SaveChangesAsync();
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
                CountryList = _repo.CountryService.GetCountries().ToList(),
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
            if (user == null)
            {
                return View("Error");
            }

            var newCountry = _repo.CountryService.GetCountryById(model.CountryId);

            var result = user.ChangeCountry(newCountry);
            if(result.Success)
            {
                await _repo.SaveChangesAsync();
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
                MarketPlaceholder = user.Items.First(c => c.ItemType == ItemType.MarketPlaceholder).Amount,
                Food = user.Items.First(c => c.ItemType == ItemType.Food).Amount,
                Grain = user.Items.First(c => c.ItemType == ItemType.Grain).Amount
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

        private async Task<ApplicationUser> GetCurrentUserAsync()
        {
            var identityUser = await _userManager.GetUserAsync(HttpContext.User);
            return _repo.ApplicationUserService.GetApplicationUserById(identityUser.Id);
        }

        #endregion
    }
}
