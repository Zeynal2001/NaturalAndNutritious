using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace NaturalAndNutritious.Data.Entities
{
    public class AppUser : IdentityUser
    {
        public string FName { get; set; }
        public string LName { get; set; }
        public DateTime BirthDate { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsSubscribed { get; set; }
        public string? ProfilePhotoUrl { get; set; }
        [NotMapped]
        public string FullName { get => FName + " " + LName; }

        public virtual List<Order> Orders { get; set; }
        public IEnumerable<Review> Reviews { get; set; }
    }
}
