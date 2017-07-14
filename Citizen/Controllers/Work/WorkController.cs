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
using Citizen.Models.WorkViewModels;

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

        // GET: Work/Company/5
        public async Task<IActionResult> Company(int? id, string message)
        {
            if (message != null)
            {
                ViewData["StatusMessage"] = message;
            }

            if (id == null)
            {
                return NotFound();
            }

            var company = await GetCompanyByIdAsync(id.Value);
            if (company == null)
            {
                return NotFound();
            }

            var viewModel = new CompanyViewModel()
            {
                Id = company.Id,
                Name = company.Name,
                Product = company.Product,
                Source = company.Source,
                MaxEmployments = company.MaxEmployments,
                Employments = company.Employments.ToList(),
                JobOffers = company.JobOffers.ToList()
            };

            return View(viewModel);
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

        public Task<Company> GetCompanyByIdAsync(int id)
        {
            return _dbContext.Companies
                .Include(m => m.Owner)
                .Include(m => m.JobOffers)
                .Include(m => m.Employments)
                    .ThenInclude(n => n.ApplicationUser)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<Employment> GetEmploymentByIdAsync(int id)
        {
            return await _dbContext.Employments
                .Include(e => e.ApplicationUser)
                .FirstOrDefaultAsync(e => e.Id == id);
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
