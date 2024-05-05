using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalAndNutritious.Data.Entities
{
    public class Shipper : BaseEntity
    {
        public string CompanyName { get; set; }
        public string PhoneNumber { get; set; }
    }
}
