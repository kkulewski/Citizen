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

namespace Citizen.Controllers.Work
{
    [Authorize]
    public class WorkController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _dbContext;

        public WorkController(UserManager<ApplicationUser> userManager, ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }

        // GET: Work
        public async Task<IActionResult> Index(string message)
        {
            if (message != null)
            {
                ViewData["StatusMessage"] = message;
            }

            var user = await GetCurrentUserAsync();
            return View(user.Companies);
        }

        // GET: Work/CreateCompany
        public IActionResult CreateCompany(string message)
        {
            if (message != null)
            {
                ViewData["StatusMessage"] = message;
            }

            var productTypes = new List<ItemType>()
            {
                ItemType.Food,
                ItemType.Grain
            };

            var viewModel = new CreateCompanyViewModel() { ProductTypes = productTypes };
            return View(viewModel);
        }

        // POST: Work/CreateCompany
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCompany(CreateCompanyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await GetCurrentUserAsync();
            var result = user.CreateCompany(model.Name, model.Product);
            if (result.Success)
            {
                await SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { result.Message });
            }

            return RedirectToAction(nameof(CreateCompany), new { result.Message });
        }

        #region Helpers

        private async Task<ApplicationUser> GetCurrentUserAsync()
        {
            var identityUser = await _userManager.GetUserAsync(HttpContext.User);
            return await _dbContext.ApplicationUsers
                .Include(p => p.Items)
                .Include(p => p.Companies)
                .Include(p => p.Country)
                .Include(p => p.UserStorage)
                .FirstOrDefaultAsync(u => u.Id == identityUser.Id);
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
