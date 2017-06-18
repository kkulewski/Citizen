using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Citizen.Models.ManageViewModels
{
    public class ChangeCountryViewModel
    {
        [Display(Name = "Money")]
        [DataType(DataType.Currency)]
        public decimal Money { get; set; }

        [Required]
        [Display(Name = "Country ID")]
        public int CountryId { get; set; }

        [Display(Name = "Country")]
        public List<Country> CountryList { get; set; }
    }
}
