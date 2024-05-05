using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalAndNutritious.Data.Entities
{
    public class Territory : BaseEntity
    {
        public string TerritoryDescription { get; set; }
        public Guid RegionId { get; set; }
        public Region Region { get; set; }
    }
}
