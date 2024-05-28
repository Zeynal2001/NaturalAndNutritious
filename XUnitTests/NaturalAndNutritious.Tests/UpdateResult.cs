using Microsoft.AspNetCore.Identity;

namespace NaturalAndNutritious.Tests
{
    internal class UpdateResult : IdentityResult
    {
        public bool Succeeded { get; set; }
    }
}