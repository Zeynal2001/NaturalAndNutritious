using System.ComponentModel.DataAnnotations;

namespace NaturalAndNutritious.Business.Dtos.AdminPanelDtos
{
    public class CreateSubscriberDto
    {
        [Required]
        public string SubscriberEmail { get; set; }
    }
}
