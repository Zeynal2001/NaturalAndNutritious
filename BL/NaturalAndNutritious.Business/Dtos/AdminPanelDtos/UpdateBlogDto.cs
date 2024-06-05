using Microsoft.AspNetCore.Http;
using NaturalAndNutritious.Business.CustomValidations;
using System.ComponentModel.DataAnnotations;

namespace NaturalAndNutritious.Business.Dtos.AdminPanelDtos
{
    public class UpdateBlogDto
    {
        public Guid Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string ShortDescription { get; set; }
        [Required]
        public string Description { get; set; }
        public string BlogPhotoUrl { get; set; }
        [FileValidator(AcceptedTypes = ".png, .jpg, .jpeg, .svg")]
        public IFormFile? BlogPhoto { get; set; }
    }
}
