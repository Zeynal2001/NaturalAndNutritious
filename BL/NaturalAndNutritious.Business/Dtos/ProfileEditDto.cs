using Microsoft.AspNetCore.Http;
using NaturalAndNutritious.Business.CustomValidations;
using System.ComponentModel.DataAnnotations;

namespace NaturalAndNutritious.Business.Dtos
{
    public class ProfileEditDto
    {
        public string Id { get; set; }
        [Required]
        public string NickName { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [CustomBirthDateValidator]
        public DateTime BirthDate { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string? ProfilePhotoUrl { get; set; }
        [FileValidator(AcceptedTypes = ".png, .jpg, .jpeg, .svg")]
        public IFormFile? ProfilePhoto { get; set; }
    }
}
