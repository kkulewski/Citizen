using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Citizen.Models
{
    public abstract class Item
    {
        public string Name { get; set; }

        public int Amount { get; set; }
    }
}
