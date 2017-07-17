using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Citizen.Models
{
    public class UserStorage
    {
        [Key]
        public int Id { get; set; }

        public ApplicationUser ApplicationUser { get; set; }

        [ForeignKey("ApplicationUser")]
        public string ApplicationUserId { get; set; }

        public int Capacity { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
