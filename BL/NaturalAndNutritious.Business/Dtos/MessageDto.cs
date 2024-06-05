using System.ComponentModel.DataAnnotations;

namespace NaturalAndNutritious.Business.Dtos
{
    public class MessageDto
    {
        [Required]
        public string CustomerName { get; set; }
        [EmailAddress]
        public string CustomerEmailAddress { get; set; }
        [Required]
        public string CustomerMessage { get; set; }
    }
}