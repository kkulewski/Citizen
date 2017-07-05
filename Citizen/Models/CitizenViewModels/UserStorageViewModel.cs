using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Citizen.Models.CitizenViewModels
{
    public class UserStorageViewModel
    {
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Energy")]
        public int Energy { get; set; }

        [Display(Name = "Money")]
        [DataType(DataType.Currency)]
        public decimal Money { get; set; }

        [Display(Name = "Country")]
        public Country Country { get; set; }

        [Display(Name = "Capacity")]
        public int Capacity { get; set; }

        [Display(Name = "Capacity used")]
        public int CapacityUsed { get; set; }

        [Display(Name = "Market placeholder")]
        public int MarketPlaceholder { get; set; }

        [Display(Name = "Food")]
        public int Food { get; set; }

        [Display(Name = "Grain")]
        public int Grain { get; set; }
    }
}
