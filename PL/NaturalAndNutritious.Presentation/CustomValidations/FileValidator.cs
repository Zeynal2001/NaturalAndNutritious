using System.ComponentModel.DataAnnotations;

namespace NaturalAndNutritious.Presentation.CustomValidations
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
            if (string.IsNullOrWhiteSpace(AcceptedTypes))
            {
                return true;
            }

            if (value != null && value is IFormFile file)
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

            return false;
        }
    }
}
