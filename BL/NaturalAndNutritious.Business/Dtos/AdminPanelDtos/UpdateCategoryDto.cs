using System.ComponentModel.DataAnnotations;

namespace NaturalAndNutritious.Business.Dtos.AdminPanelDtos
{
    public class UpdateCategoryDto
    {
        public string Id { get; set; }
        [Required]
        public string CategoryName { get; set; }
    }
}
