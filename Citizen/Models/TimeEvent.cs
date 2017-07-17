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

        [Range(minimum: 1, maximum: 3600)]
        public int Tick { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime LastTrigger { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
