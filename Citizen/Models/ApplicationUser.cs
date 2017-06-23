using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Citizen.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }

        public int Energy { get; set; }

        public int EnergyRestore { get; set; }

        public decimal Money { get; set; }

        [ForeignKey("Country")]
        public int CountryId { get; set; }

        public Country Country { get; set; }

        public void EnergyRestoreEvent(int ticks)
        {
            var maxEnergy = 1000;
            var amount = 1 * ticks;
            EnergyRestore = Math.Min(EnergyRestore + amount, maxEnergy);
        }

        public void Consume(ConsumableItem item)
        {
            Energy += item.EnergyRecoverAmount;
            EnergyRestore -= item.EnergyRecoverAmount;
        }

        public bool CanRestoreEnergyAmount(int amount)
        {
            var maxEnergy = 1000;

            if (amount > EnergyRestore)
                return false;

            if (amount + Energy > maxEnergy)
                return false;

            return true;
        }
    }
}
