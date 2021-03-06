﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Citizen.Models
{
    public class JobOffer
    {
        [Key]
        public int Id { get; set; }

        public Company Company { get; set; }

        [ForeignKey("Company")]
        public int CompanyId { get; set; }

        public decimal Salary { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
