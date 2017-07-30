using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace Citizen.Models
{
    [NotMapped]
    public class GameSettings
    {
        public static int EnergyMax = 100;

        public static int FoodEnergyRestore = 10;

        public static int EnergyRestoreEventTickAmount = 1;

        public static decimal StorageExtensionCostDivisor = 5.00M;

        public static int StorageExtensionCapacity = 1000;

        public static int WorkEnergyCost = 10;

        public static int WorkExperienceGain = 10;

        public static TimeSpan WorkInterval = TimeSpan.FromHours(24);

        public static decimal CountryChangeCost = 5.00M;

        public static decimal CompanyCost = 1000.00M;

        public static int CompanyMaxWorkers = 10;

        public static int CompanyWorkProduct = 10;

        public static int CompanyWorkSource = 10;

        public static int DefaultExperience = 0;

        public static decimal DefaultMoney = 100.00M;

        public static int DefaultStorageCapacity = 1000;

        public static int DefaultFoodAmount = 5;

        public static int DefaultGrainAmount = 0;

        public static int DefaultMarketPlaceholderAmount = 0;

        public static DateTime DefaultLastWorked = DateTime.MinValue;

        public const string DataConcurrencyOk = "Changes saved";

        public const string DataConcurrencyError = "Data has been modified by someone else. Try again.";
    }
}
