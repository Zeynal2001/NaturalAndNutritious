using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace NaturalAndNutritious.Business.CustomValidations
{
    public class FileValidator : ValidationAttribute
    {
        public FileValidator()
        {
            ErrorMessage = "Invalid file";
        }

        public string? AcceptedTypes { get; set; }

        public override bool IsValid(object? value)
        {
            //if (string.IsNullOrWhiteSpace(AcceptedTypes))
            //{
            //    return true;
            //}
            //value != null &&
            if (value is IFormFile file)
            {
                var extension = Path.GetExtension(file.FileName);
                var accepteds = AcceptedTypes.Split(',');

                foreach (var accepted in accepteds)
                {
                    if (accepted.Trim().ToLower() == extension.Trim().ToLower())
                    {
                        return true;
                    }
                }
            }
            else
            {
                return true;
            }

            return false;
        }
    }
}
