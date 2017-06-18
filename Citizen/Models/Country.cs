using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Citizen.Models
{
    public class Country
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(50, MinimumLength = 2)]
        public string Name { get; set; }

        [StringLength(50, MinimumLength = 2)]
        public string Capital { get; set; }

        [StringLength(3, MinimumLength = 3)]
        public string CurrencyCode { get; set; }

        public virtual ICollection<ApplicationUser> Citizens { get; set; }
    }
}
