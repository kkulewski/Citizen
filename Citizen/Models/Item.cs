using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Citizen.Models
{
    public abstract class Item
    {
        public ItemType ItemType { get; set; }

        public int Amount { get; set; }
    }
}
