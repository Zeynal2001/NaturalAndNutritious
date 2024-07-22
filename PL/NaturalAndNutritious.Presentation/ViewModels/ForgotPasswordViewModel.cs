using System.ComponentModel.DataAnnotations;

namespace NaturalAndNutritious.Presentation.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
