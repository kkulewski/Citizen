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
            if(user.Employment == null)
            {
                return View(null);
            }

            var employment = await GetEmploymentByIdAsync(user.Employment.Id);
            if(employment == null)
            {

                return View(null);
            }


            return View(employment);
        }


        // GET: Work/Companies
        public async Task<IActionResult> Companies(string message)
        {
            if (message != null)
            {
                ViewData["StatusMessage"] = message;
            }

            var user = await GetCurrentUserAsync();
            return View(user.Companies);
        }

        // GET: Work/JobOffers
        public async Task<IActionResult> JobOffers(string message)
        {
            if (message != null)
            {
                ViewData["StatusMessage"] = message;
            }

            var user = await GetCurrentUserAsync();
            var jobOffers = await GetJobOfferListAsync();

            return View(jobOffers.OrderByDescending(c => c.Salary));
        }

        // POST: Work/JobOffers/Apply
        public async Task<IActionResult> JobApply(int id)
        {
            var user = await GetCurrentUserAsync();
            var jobOffer = await GetJobOfferByIdAsync(id);

            var result = user.JobApply(jobOffer);
            if (result.Success)
            {
                await SaveChangesAsync();
            }

            return RedirectToAction(nameof(JobOffers), new { message = result.Message });
        }

        // POST: Work/JobResign
        public async Task<IActionResult> JobResign(int id)
        {
            var user = await GetCurrentUserAsync();
            var employment = await GetEmploymentByIdAsync(id);

            var result = user.JobResign(employment);
            if (result.Success)
            {
                await SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index), new { message = result.Message });
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
                return RedirectToAction(nameof(Companies), new { result.Message });
            }

            return RedirectToAction(nameof(CreateCompany), new { result.Message });
        }

        // GET: Work/DeleteCompany/5
        public async Task<IActionResult> DeleteCompany(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await GetCompanyByIdAsync(id.Value);
            if (company == null)
            {
                return NotFound();
            }

            var viewModel = new DeleteCompanyViewModel()
            {
                Id = company.Id,
                Name = company.Name,
                Product = company.Product,
                Source = company.Source
            };

            return View(viewModel);
        }

        // POST: Work/DeleteCompany/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCompany(DeleteCompanyViewModel model)
        {
            var user = await GetCurrentUserAsync();
            var company = await GetCompanyByIdAsync(model.Id);
            var result = user.DeleteCompany(company);
            if (result.Success)
            {
                await SaveChangesAsync();
            }

            return RedirectToAction(nameof(Companies), new { result.Message });
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

        // POST: Work/Company/5/FireWorker
        public async Task<IActionResult> FireWorker(int companyId, int employmentId)
        {
            var user = await GetCurrentUserAsync();
            var company = await GetCompanyByIdAsync(companyId);
            var employment = await GetEmploymentByIdAsync(employmentId);

            var result = user.FireWorker(company, employment);
            if (result.Success)
            {
                await SaveChangesAsync();
            }

            return RedirectToAction(nameof(Company), new { id = companyId, message = result.Message });
        }

        // GET: Work/Company/5/AddJobOffer
        public async Task<IActionResult> AddJobOffer(string message, int companyId)
        {
            if (message != null)
            {
                ViewData["StatusMessage"] = message;
            }

            var company = await GetCompanyByIdAsync(companyId);
            if (company == null)
            {
                return NotFound();
            }

            var viewModel = new AddJobOfferViewModel()
            {
                Company = company,
                CompanyId = company.Id
            };

            return View(viewModel);
        }

        // POST: /Work/Company/5/AddJobOffer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddJobOffer(AddJobOfferViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await GetCurrentUserAsync();
            var company = await GetCompanyByIdAsync(model.CompanyId);

            var result = user.AddJobOffer(company, model.Salary);
            if (result.Success)
            {
                await SaveChangesAsync();
                return RedirectToAction(nameof(Company), new { id = model.CompanyId, message = result.Message });
            }

            return RedirectToAction(nameof(AddJobOffer), new { companyId = model.CompanyId, message = result.Message });
        }

        // POST: Work/Company/5/DeleteJobOffer
        public async Task<IActionResult> DeleteJobOffer(int companyId, int jobOfferId)
        {
            var user = await GetCurrentUserAsync();
            var company = await GetCompanyByIdAsync(companyId);
            var jobOffer = await GetJobOfferByIdAsync(jobOfferId);

            var result = user.DeleteJobOffer(company, jobOffer);
            if (result.Success)
            {
                await SaveChangesAsync();
            }

            return RedirectToAction(nameof(Company), new { id = companyId, message = result.Message });
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
                .Include(p => p.Employment)
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

        public async Task<JobOffer> GetJobOfferByIdAsync(int id)
        {
            return await _dbContext.JobOffers
                .Include(e => e.Company)
                    .ThenInclude(c => c.JobOffers)
                .Include(e => e.Company)
                    .ThenInclude(d => d.Employments)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<List<JobOffer>> GetJobOfferListAsync()
        {
            return await _dbContext.JobOffers
                .Include(e => e.Company)
                    .ThenInclude(c => c.Owner)
                .ToListAsync();
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
