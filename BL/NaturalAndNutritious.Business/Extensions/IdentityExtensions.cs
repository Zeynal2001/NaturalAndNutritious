using Microsoft.AspNetCore.Identity;

namespace NaturalAndNutritious.Business.Extensions
{
    public static class IdentityExtensions
    {
        public static string ErrorsToString(this IEnumerable<IdentityError> errors)
        {
            return string.Join("\n", errors.Select(err => err.Description).ToList());
        }
    }
}
