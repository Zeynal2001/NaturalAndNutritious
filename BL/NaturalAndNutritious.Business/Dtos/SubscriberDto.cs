using System.ComponentModel.DataAnnotations;

namespace NaturalAndNutritious.Business.Dtos
{
    public class SubscriberDto
    {
        [EmailAddress]
        public string SubscriberEmail { get; set; }
    }
}
