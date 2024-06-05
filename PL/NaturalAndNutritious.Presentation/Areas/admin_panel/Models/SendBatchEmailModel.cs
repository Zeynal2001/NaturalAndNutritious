using NaturalAndNutritious.Business.CustomValidations;
using System.ComponentModel.DataAnnotations;

namespace NaturalAndNutritious.Presentation.Areas.admin_panel.Models
{
    public class SendBatchEmailModel
    {
        [EmailListValidator]
        public List<string> Emails { get; set; }

        [Required]
        [MaxLength(80, ErrorMessage = "Too long for subject name")]
        public string Subject { get; set; }
        [Required]
        public string Message { get; set; }
    }
}
