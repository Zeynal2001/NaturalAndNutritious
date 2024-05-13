using System.ComponentModel.DataAnnotations;

namespace NaturalAndNutritious.Business.Dtos
{
    public class LoginDto
    {
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public bool RememberMe { get; set; }
        public DateTime sdsd { get; set; }
    }
}
