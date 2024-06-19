using System.ComponentModel.DataAnnotations;

namespace NaturalAndNutritious.Business.Dtos.AdminPanelDtos
{
    public class ChangeOrderStatusDto
    {
        public string Id { get; set; }
        public string? CurrentStatus { get; set; }
        [Required]
        public string SelectedStatus { get; set; }
    }
}
