using System.ComponentModel.DataAnnotations.Schema;

namespace Citizen.Models
{
    [NotMapped]
    public class GameSettings
    {
        public static int EnergyMax = 100;

        public static int FoodEnergyRestore = 10;

        public static int EnergyRestoreEventTickAmount = 1;

        public static decimal CountryChangeCost = 5.00M;

        public static int DefaultStorageCapacity = 1000;

        public static int DefaultFoodAmount = 5;

        public static int DefaultGrainAmount = 0;
    }
}
