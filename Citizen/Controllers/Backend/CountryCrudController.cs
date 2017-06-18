using System.Linq;
using System.Threading.Tasks;
using Citizen.Data;
using Citizen.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Citizen.Controllers.Backend
{
    [Authorize]
    public class CountryCrudController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CountryCrudController(ApplicationDbContext context)
        {
            _context = context;    
        }

        // GET: Country
        public async Task<IActionResult> Index()
        {
            return View("~/Views/Backend/CountryCrud/Index.cshtml", await _context.Country.ToListAsync());
        }

        // GET: Country/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var country = await _context.Country.SingleOrDefaultAsync(m => m.Id == id);
            if (country == null)
            {
                return NotFound();
            }

            return View("~/Views/Backend/CountryCrud/Details.cshtml", country);
        }

        // GET: Country/Create
        public IActionResult Create()
        {
            return View("~/Views/Backend/CountryCrud/Create.cshtml");
        }

        // POST: Country/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Capital,CurrencyCode")] Country country)
        {
            if (ModelState.IsValid)
            {
                _context.Add(country);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View("~/Views/Backend/CountryCrud/Create.cshtml", country);
        }

        // GET: Country/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var country = await _context.Country.SingleOrDefaultAsync(m => m.Id == id);
            if (country == null)
            {
                return NotFound();
            }
            return View("~/Views/Backend/CountryCrud/Edit.cshtml", country);
        }

        // POST: Country/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Capital,CurrencyCode")] Country country)
        {
            if (id != country.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(country);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CountryExists(country.Id))
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
            return View("~/Views/Backend/CountryCrud/Edit.cshtml", country);
        }

        // GET: Country/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var country = await _context.Country.SingleOrDefaultAsync(m => m.Id == id);
            if (country == null)
            {
                return NotFound();
            }

            return View("~/Views/Backend/CountryCrud/Delete.cshtml", country);
        }

        // POST: Country/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var country = await _context.Country.SingleOrDefaultAsync(m => m.Id == id);
            _context.Country.Remove(country);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool CountryExists(int id)
        {
            return _context.Country.Any(e => e.Id == id);
        }
    }
}
