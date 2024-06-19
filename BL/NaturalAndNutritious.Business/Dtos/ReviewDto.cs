using System.ComponentModel.DataAnnotations;

namespace NaturalAndNutritious.Business.Dtos
{
    public class ReviewDto
    {
        public Guid ProductId { get; set; }
        public string UserName { get; set; } 
        public string UserEmail { get; set; }
        public string ReviewText { get; set; }
        public AppUserDto User { get; set; }
        //[Required]
        //[Range(0, 5, ErrorMessage = "Rating must be between 0 and 5 stars.")]
        public int Rating { get; set; }
        public DateTime ReviewDate { get; set; }
    }
}
