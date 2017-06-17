using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Citizen.Models.ManageViewModels
{
    public class IndexViewModel
    {
        public bool HasPassword { get; set; }

        public bool BrowserRemembered { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Money")]
        [DataType(DataType.Currency)]
        public decimal Money { get; set; }
    }
}
