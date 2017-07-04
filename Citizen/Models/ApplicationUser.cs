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

        public UserStorage UserStorage { get; set; }

        public void EnergyRestoreEvent(int ticks)
        {
            var amount = GameSettings.EnergyRestoreEventTickAmount * ticks;
            EnergyRestore = Math.Min(EnergyRestore + amount, GameSettings.MaxEnergy);
        }

        public bool Eat(ConsumableItem item)
        {
            if (!CanEat(item))
                return false;

            var energyRestored = Math.Min(item.EnergyRecoverAmount, EnergyRestore);
            Energy = Math.Min(Energy + energyRestored, GameSettings.MaxEnergy);
            EnergyRestore -= energyRestored;
            item.Amount -= 1;

            return true;
        }

        private bool CanEat(ConsumableItem item)
        {
            if (item.Amount <= 0)
                return false;

            if (Energy == GameSettings.MaxEnergy)
                return false;

            if (EnergyRestore <= 0)
                return false;

            return true;
        }
    }
}
