using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalAndNutritious.Business.Dtos.AdminPanelDtos
{
    public class AllProductsDto
    {
        public string Id { get; set; }
        public string ProductName { get; set; }
        public string ProductImageUrl { get; set; }
        public double ProductPrice { get; set; }
        public int UnitsInStock { get; set; }
        public int UnitsOnOrder { get; set; }
        public int ReOrderLevel { get; set; }
        public bool Discontinued { get; set; }
    }
}
