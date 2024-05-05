using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalAndNutritious.Data.Entities
{
    public class Review : BaseEntity
    {
        public string ReviewText { get; set; }
        public int Rating { get; set; }
        public DateTime ReviewDate { get; set; }

        public Guid? ProductId { get; set; }

        public AppUser AppUser { get; set; }
        public Product Product { get; set; }
    }
}
