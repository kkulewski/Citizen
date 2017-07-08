using System.Threading.Tasks;
using Citizen.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Citizen.Models;
using Citizen.Models.ManageViewModels;
using Microsoft.EntityFrameworkCore;

namespace Citizen.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _dbContext;

        public ManageController(
          UserManager<ApplicationUser> userManager,
          SignInManager<ApplicationUser> signInManager,
          ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _dbContext = dbContext;
        }
        
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

            var model = new IndexViewModel
            {
                HasPassword = await _userManager.HasPasswordAsync(user),
                BrowserRemembered = await _signInManager.IsTwoFactorClientRememberedAsync(user),
                Name = user.Name
            };

            return View(model);
        }
        
        // GET: /Manage/ChangePassword
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }
        
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction(nameof(Index), new { Message = "Password changed succesfully." });
                }
                AddErrors(result);
                return View(model);
            }
            return RedirectToAction(nameof(Index), new { Message = "Error." });
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

        #endregion
    }
}
