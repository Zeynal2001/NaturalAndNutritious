using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalAndNutritious.Data.Entities
{
    public class SubCategory : BaseEntity
    {
        public string SubCategoryName { get; set; }
        public Guid? CategoryId { get; set; }
        public Category Category { get; set; }
        public IEnumerable<Product> Products { get; set; }
    }
}
