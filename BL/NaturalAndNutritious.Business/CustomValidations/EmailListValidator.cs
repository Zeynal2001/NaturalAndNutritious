using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace NaturalAndNutritious.Business.CustomValidations
{
    public class EmailListValidator : ValidationAttribute
    {
        public EmailListValidator()
        {
            ErrorMessage = "Some of mails aren't correct.";
        }

        public override bool IsValid(object? value)
        {
            List<string>? mails = (List<string>?) value;

            if (value == null || mails == null)
            {
                return false;
            }

            var result = mails.All(m => Regex.IsMatch(m, @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$"));

            return result;
        }
    }
}
