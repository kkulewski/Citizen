using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Citizen.Models
{
    public class Country
    {
        [StringLength(2, MinimumLength = 50)]
        public string Name;

        [StringLength(2, MinimumLength = 50)]
        public string Capital;

        [StringLength(3, MinimumLength = 3)]
        public string CurrencyCode;
    }
}
