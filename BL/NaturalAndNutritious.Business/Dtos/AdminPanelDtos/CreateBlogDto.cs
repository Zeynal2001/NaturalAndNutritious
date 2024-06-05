using Microsoft.AspNetCore.Http;
using NaturalAndNutritious.Business.CustomValidations;
using System.ComponentModel.DataAnnotations;

namespace NaturalAndNutritious.Business.Dtos.AdminPanelDtos
{
    public class CreateBlogDto
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string ShortDescription { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        [FileValidator(AcceptedTypes = ".png, .jpg, .jpeg, .svg")]
        public IFormFile BlogPhoto { get; set; }
    }
}
