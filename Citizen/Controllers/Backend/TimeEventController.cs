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
            
            var dateDiffInSeconds = (DateTime.Now - timeEvent.LastTrigger).TotalSeconds;

            if (dateDiffInSeconds >= timeEvent.Tick)
            {
                // do action associated with this event


                // TODO: remove hardcoded events
                // update Event lastTrigger date
                timeEvent.LastTrigger = DateTime.Now;
                // count event ticks (how many times should be fired) using date-diff
                int ticks = (int) (dateDiffInSeconds / timeEvent.Tick);
                // fire EnergyRestoreEvent for each User
                await _context.ApplicationUsers.ForEachAsync(c => c.EnergyRestoreEvent(ticks));
                await _context.SaveChangesAsync();

                return Ok(string.Format("EVENT FIRED - {0} TICKS", ticks));
            }

            return Ok("EVENT NOT FIRED");
        }

    }
}