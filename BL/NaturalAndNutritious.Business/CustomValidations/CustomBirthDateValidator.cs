using System.ComponentModel.DataAnnotations;

namespace NaturalAndNutritious.Business.CustomValidations
{
    public class CustomBirthDateValidator : ValidationAttribute
    {
        public CustomBirthDateValidator()
        {
            ErrorMessage = "The date of birth must be between 1950 and today.";
        }

        public override bool IsValid(object value)
        {
            if (value is DateTime dateTime)
            {
                return dateTime >= new DateTime(1950, 1, 1) && dateTime <= DateTime.Today;
            }
            else
            {
                return false;
            }

            //var dateTime = (DateTime)value;

            //return dateTime >= new DateTime(1950, 1, 1) && dateTime <= DateTime.Today;
        }
    }
}
