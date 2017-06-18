using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Citizen.Data;
using Citizen.Models;

namespace Citizen.Controllers.Backend
{
    [Produces("application/json")]
    [Route("api/TimeEvent")]
    public class TimeEventController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TimeEventController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/TimeEvent
        [HttpGet]
        public IEnumerable<TimeEvent> GetTimeEvents()
        {
            return _context.TimeEvents;
        }

        // GET: api/TimeEvent/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTimeEvent([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var timeEvent = await _context.TimeEvents.SingleOrDefaultAsync(m => m.Id == id);

            if (timeEvent == null)
            {
                return NotFound();
            }
            
            var dateDiffInSeconds = (timeEvent.LastTrigger - DateTime.Now).TotalSeconds;

            if (dateDiffInSeconds >= timeEvent.Tick)
            {
                // do action associated with this event


                // TODO: remove hardcoded events
                // update Event lastTrigger date
                timeEvent.LastTrigger = DateTime.Now;
                // update EnergyRestore
                int ticks = (int) (dateDiffInSeconds / timeEvent.Tick);
                int energyToAdd =+ 1 * ticks;
                await _context.ApplicationUsers.ForEachAsync(c => c.EnergyRestore += energyToAdd);

                _context.SaveChanges();
                return Ok(timeEvent);
            }

            return Ok(timeEvent);
        }

        // PUT: api/TimeEvent/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTimeEvent([FromRoute] int id, [FromBody] TimeEvent timeEvent)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != timeEvent.Id)
            {
                return BadRequest();
            }

            _context.Entry(timeEvent).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TimeEventExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/TimeEvent
        [HttpPost]
        public async Task<IActionResult> PostTimeEvent([FromBody] TimeEvent timeEvent)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.TimeEvents.Add(timeEvent);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTimeEvent", new { id = timeEvent.Id }, timeEvent);
        }

        // DELETE: api/TimeEvent/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTimeEvent([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var timeEvent = await _context.TimeEvents.SingleOrDefaultAsync(m => m.Id == id);
            if (timeEvent == null)
            {
                return NotFound();
            }

            _context.TimeEvents.Remove(timeEvent);
            await _context.SaveChangesAsync();

            return Ok(timeEvent);
        }

        private bool TimeEventExists(int id)
        {
            return _context.TimeEvents.Any(e => e.Id == id);
        }
    }
}