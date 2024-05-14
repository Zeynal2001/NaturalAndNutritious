using Microsoft.AspNetCore.Http;
using NaturalAndNutritious.Business.CustomValidations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        [Required]
        public string ProfilePhotoUrl { get; set; }
        [Required]
        [FileValidator(AcceptedTypes = ".png, .jpg, .jpeg, .svg")]
        public IFormFile? ProfilePhoto { get; set; }
    }
}
