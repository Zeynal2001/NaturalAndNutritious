using System.ComponentModel.DataAnnotations;

namespace NaturalAndNutritious.Presentation.Areas.admin_panel.Models
{
    public class ReplyModel
    {
        public Guid Id { get; set; }
        [EmailAddress]
        public string Recipient { get; set; }
        [Required]
        [MaxLength(80, ErrorMessage = "Too long for subject name.")]
        public string Subject { get; set; }
        [Required]
        public string Message { get; set; }
    }
}
