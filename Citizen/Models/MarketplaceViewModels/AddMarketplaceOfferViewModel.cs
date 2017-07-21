using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Citizen.Models.MarketplaceViewModels
{
    public class AddMarketplaceOfferViewModel
    {
        [Required]
        [Display(Name = "Item")]
        public ItemType ItemType { get; set; }

        public IEnumerable<ItemType> ItemTypes { get; set; }

        [Required]
        public int Amount { get; set; }

        [Required]
        public decimal Price { get; set; }
    }
}
