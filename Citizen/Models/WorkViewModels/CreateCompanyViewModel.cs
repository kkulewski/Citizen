using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Citizen.Models.MarketplaceViewModels
{
    public class CreateCompanyViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public ItemType CompanyType { get; set; }

        public IEnumerable<ItemType> CompanyTypes { get; set; }
    }
}
