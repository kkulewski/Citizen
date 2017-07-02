using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Citizen.Models.CitizenViewModels
{
    public class ChangeCountryViewModel
    {
        [Display(Name = "Money")]
        [DataType(DataType.Currency)]
        public decimal Money { get; set; }

        [Required]
        [Display(Name = "Country")]
        public int CountryId { get; set; }

        public Country Country { get; set; }

        [Required]
        [Display(Name = "Country change cost")]
        [DataType(DataType.Currency)]
        public decimal CountryChangeCost { get; set; }

        [Display(Name = "Country")]
        public List<Country> CountryList { get; set; }
    }
}
