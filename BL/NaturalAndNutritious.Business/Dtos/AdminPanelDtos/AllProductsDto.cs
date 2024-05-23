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
        public string? ProductImageUrl { get; set; }
        public string ProductName { get; set; }
        public double ProductPrice { get; set; }
        public int UnitsInStock { get; set; }
        public int UnitsOnOrder { get; set; }
        public int ReOrderLevel { get; set; }
        public bool Discontinued { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string Supplier { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public double? Discount { get; set; }
        public string? DiscountType { get; set; }
    }
}
