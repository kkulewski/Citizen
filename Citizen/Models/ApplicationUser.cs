﻿using System;
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

        public bool Consume(ConsumableItem item)
        {
            if (!RestoreEnergy(item.EnergyRecoverAmount))
            {
                return false;
            }

            item.Amount -= 1;
            return true;
        }

        public bool RestoreEnergy(int amount)
        {
            if (!CanRestoreEnergyAmount(amount))
            {
                return false;
            }

            Energy += amount;
            EnergyRestore -= amount;
            return true;
        }

        private bool CanRestoreEnergyAmount(int amount)
        {
            if (amount > EnergyRestore)
            {
                return false;
            }

            if (amount + Energy > GameSettings.MaxEnergy)
            {
                return false;
            }

            return true;
        }
    }
}
