using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Citizen.Models
{
    public class Employment
    {
        [Key]
        public int Id { get; set; }

        public Company Company { get; set; }

        [ForeignKey("Company")]
        public int CompanyId { get; set; }

        public ApplicationUser ApplicationUser { get; set; }

        [ForeignKey("ApplicationUser")]
        public string ApplicationUserId { get; set; }

        public decimal Salary { get; set; }

        public int DaysWorker { get; set; }
    }
}
