using System.ComponentModel.DataAnnotations;

namespace NaturalAndNutritious.Business.Dtos.AdminPanelDtos
{
    public class UpdateSubCategoryDto
    {
        public string Id { get; set; }
        [Required]
        public string SubCategoryName { get; set; }
    }
}
