using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Citizen.Models
{
    public class Company
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public ItemType Type { get; set; }

        public ApplicationUser Owner { get; set; }

        [ForeignKey("ApplicationUser")]
        public string OwnerId { get; set; }

        public int MaxEmployments { get; set; }

        public virtual ICollection<Employment> Employments { get; set; }

        public virtual ICollection<JobOffer> JobOffers { get; set; }
    }
}
