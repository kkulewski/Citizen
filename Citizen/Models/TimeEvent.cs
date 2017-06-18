using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Citizen.Models
{
    public class TimeEvent
    {
        [Key]
        public int Id { get; set; }

        [StringLength(200, MinimumLength = 1)]
        public string Name { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime LastTrigger { get; set; }
    }
}
