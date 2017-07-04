using System.ComponentModel.DataAnnotations.Schema;

namespace Citizen.Models
{
    [NotMapped]
    public class GameSettings
    {
        public static int MaxEnergy = 100;

        public static int FoodEnergyRecover = 10;

        public static int EnergyRestoreEventTickAmount = 1;

        public static decimal CountryChangeCost = 5.00M;
    }
}
