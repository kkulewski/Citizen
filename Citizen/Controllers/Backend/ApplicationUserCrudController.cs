using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Citizen.Data;
using Citizen.Models;
using Microsoft.AspNetCore.Authorization;

namespace Citizen.Controllers.Backend
{
    [Authorize]
    public class ApplicationUserCrudController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ApplicationUserCrudController(ApplicationDbContext context)
        {
            _context = context;    
        }

        // GET: ApplicationUserCrud
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ApplicationUsers.Include(a => a.Country);
            return View("~/Views/Backend/ApplicationUserCrud/Index.cshtml", await applicationDbContext.ToListAsync());
        }

        // GET: ApplicationUserCrud/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationUser = await _context.ApplicationUsers
                .Include(a => a.Country)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (applicationUser == null)
            {
                return NotFound();
            }

            return View("~/Views/Backend/ApplicationUserCrud/Details.cshtml", applicationUser);
        }

        // GET: ApplicationUserCrud/Create
        public IActionResult Create()
        {
            ViewData["CountryId"] = new SelectList(_context.Country, "Id", "Name");
            return View("~/Views/Backend/ApplicationUserCrud/Create.cshtml");
        }

        // POST: ApplicationUserCrud/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Energy,Money,CountryId,Id,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount")] ApplicationUser applicationUser)
        {
            if (ModelState.IsValid)
            {
                _context.Add(applicationUser);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewData["CountryId"] = new SelectList(_context.Country, "Id", "Name", applicationUser.CountryId);
            return View("~/Views/Backend/ApplicationUserCrud/Create.cshtml", applicationUser);
        }

        // GET: ApplicationUserCrud/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationUser = await _context.ApplicationUsers.SingleOrDefaultAsync(m => m.Id == id);
            if (applicationUser == null)
            {
                return NotFound();
            }
            ViewData["CountryId"] = new SelectList(_context.Country, "Id", "Name", applicationUser.CountryId);
            return View("~/Views/Backend/ApplicationUserCrud/Edit.cshtml", applicationUser);
        }

        // POST: ApplicationUserCrud/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Name,Energy,Money,CountryId,Id,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount")] ApplicationUser applicationUser)
        {
            if (id != applicationUser.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(applicationUser);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ApplicationUserExists(applicationUser.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            ViewData["CountryId"] = new SelectList(_context.Country, "Id", "Name", applicationUser.CountryId);
            return View("~/Views/Backend/ApplicationUserCrud/Edit.cshtml", applicationUser);
        }

        // GET: ApplicationUserCrud/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationUser = await _context.ApplicationUsers
                .Include(a => a.Country)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (applicationUser == null)
            {
                return NotFound();
            }

            return View("~/Views/Backend/ApplicationUserCrud/Delete.cshtml", applicationUser);
        }

        // POST: ApplicationUserCrud/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var applicationUser = await _context.ApplicationUsers.SingleOrDefaultAsync(m => m.Id == id);
            _context.ApplicationUsers.Remove(applicationUser);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool ApplicationUserExists(string id)
        {
            return _context.ApplicationUsers.Any(e => e.Id == id);
        }
    }
}
