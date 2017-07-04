﻿using System.ComponentModel.DataAnnotations.Schema;

namespace Citizen.Models
{
    [NotMapped]
    public class GameSettings
    {
        public static int MaxEnergy = 100;

        public static int FoodEnergyRecover = 10;
    }
}
