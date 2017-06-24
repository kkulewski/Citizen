using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Citizen.Models.Items
{
    public abstract class ConsumableItem : Item
    {
        public int EnergyRecoverAmount { get; set; }
    }
}
