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
    public class TimeEventCrudController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TimeEventCrudController(ApplicationDbContext context)
        {
            _context = context;    
        }

        // GET: TimeEventCrud
        public async Task<IActionResult> Index()
        {
            return View("~/Views/Backend/TimeEventCrud/Index.cshtml", await _context.TimeEvents.ToListAsync());
        }

        // GET: TimeEventCrud/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var timeEvent = await _context.TimeEvents
                .SingleOrDefaultAsync(m => m.Id == id);
            if (timeEvent == null)
            {
                return NotFound();
            }

            return View("~/Views/Backend/TimeEventCrud/Details.cshtml", timeEvent);
        }

        // GET: TimeEventCrud/Create
        public IActionResult Create()
        {
            return View("~/Views/Backend/TimeEventCrud/Create.cshtml");
        }

        // POST: TimeEventCrud/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,LastTrigger")] TimeEvent timeEvent)
        {
            if (ModelState.IsValid)
            {
                _context.Add(timeEvent);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View("~/Views/Backend/TimeEventCrud/Create.cshtml", timeEvent);
        }

        // GET: TimeEventCrud/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var timeEvent = await _context.TimeEvents.SingleOrDefaultAsync(m => m.Id == id);
            if (timeEvent == null)
            {
                return NotFound();
            }
            return View("~/Views/Backend/TimeEventCrud/Edit.cshtml", timeEvent);
        }

        // POST: TimeEventCrud/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,LastTrigger")] TimeEvent timeEvent)
        {
            if (id != timeEvent.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(timeEvent);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TimeEventExists(timeEvent.Id))
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
            return View("~/Views/Backend/TimeEventCrud/Edit.cshtml", timeEvent);
        }

        // GET: TimeEventCrud/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var timeEvent = await _context.TimeEvents
                .SingleOrDefaultAsync(m => m.Id == id);
            if (timeEvent == null)
            {
                return NotFound();
            }

            return View("~/Views/Backend/TimeEventCrud/Delete.cshtml", timeEvent);
        }

        // POST: TimeEventCrud/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var timeEvent = await _context.TimeEvents.SingleOrDefaultAsync(m => m.Id == id);
            _context.TimeEvents.Remove(timeEvent);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool TimeEventExists(int id)
        {
            return _context.TimeEvents.Any(e => e.Id == id);
        }
    }
}
