using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Citizen.Models
{
    public class MarketplaceOffer
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("ApplicationUser")]
        public string ApplicationUserId { get; set; }

        public ApplicationUser ApplicationUser { get; set; }

        public ItemType ItemType { get; set; }

        public int Amount { get; set; }

        public decimal Price { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
