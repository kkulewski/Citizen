using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Citizen.Data;
using Citizen.Models;

namespace Citizen.Controllers.Marketplace
{
    public class MarketplaceController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MarketplaceController(ApplicationDbContext context)
        {
            _context = context;    
        }

        // GET: Marketplace
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.MarketplaceOffers.Include(m => m.ApplicationUser);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Marketplace
        public async Task<IActionResult> Offers(ItemType? itemType)
        {
            //var applicationDbContext = _context.MarketplaceOffers.Include(m => m.ApplicationUser && m.ItemType == itemType);
            var offers = _context.MarketplaceOffers.Where(offer => offer.ItemType == itemType);
            return View("~/Views/Marketplace/Index.cshtml", await offers.ToListAsync());
        }

        // GET: Marketplace/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var marketplaceOffer = await _context.MarketplaceOffers
                .Include(m => m.ApplicationUser)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (marketplaceOffer == null)
            {
                return NotFound();
            }

            return View(marketplaceOffer);
        }

        // GET: Marketplace/Create
        public IActionResult Create()
        {
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Name");

            IEnumerable<ItemType> itemTypes = new List<ItemType>()
            {
                ItemType.Food,
                ItemType.Grain
            };

            ViewData["ItemType"] = new SelectList(itemTypes);

            return View();
        }

        // POST: Marketplace/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ApplicationUserId,ItemType,Amount,Price")] MarketplaceOffer marketplaceOffer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(marketplaceOffer);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", marketplaceOffer.ApplicationUserId);
            return View(marketplaceOffer);
        }

        // GET: Marketplace/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var marketplaceOffer = await _context.MarketplaceOffers.SingleOrDefaultAsync(m => m.Id == id);
            if (marketplaceOffer == null)
            {
                return NotFound();
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", marketplaceOffer.ApplicationUserId);
            return View(marketplaceOffer);
        }

        // POST: Marketplace/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ApplicationUserId,ItemType,Amount,Price")] MarketplaceOffer marketplaceOffer)
        {
            if (id != marketplaceOffer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(marketplaceOffer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MarketplaceOfferExists(marketplaceOffer.Id))
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
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", marketplaceOffer.ApplicationUserId);
            return View(marketplaceOffer);
        }

        // GET: Marketplace/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var marketplaceOffer = await _context.MarketplaceOffers
                .Include(m => m.ApplicationUser)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (marketplaceOffer == null)
            {
                return NotFound();
            }

            return View(marketplaceOffer);
        }

        // POST: Marketplace/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var marketplaceOffer = await _context.MarketplaceOffers.SingleOrDefaultAsync(m => m.Id == id);
            _context.MarketplaceOffers.Remove(marketplaceOffer);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool MarketplaceOfferExists(int id)
        {
            return _context.MarketplaceOffers.Any(e => e.Id == id);
        }
    }
}
