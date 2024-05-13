using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
