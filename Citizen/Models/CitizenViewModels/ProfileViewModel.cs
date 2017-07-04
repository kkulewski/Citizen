using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Citizen.Models.CitizenViewModels
{
    public class ProfileViewModel
    {
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Energy")]
        public int Energy { get; set; }

        [Display(Name = "Energy to restore")]
        public int EnergyRestore { get; set; }

        [Display(Name = "Money")]
        [DataType(DataType.Currency)]
        public decimal Money { get; set; }

        [Display(Name = "Country")]
        public Country Country { get; set; }
    }
}