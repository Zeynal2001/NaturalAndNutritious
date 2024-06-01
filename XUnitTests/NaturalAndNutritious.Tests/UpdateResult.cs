using Microsoft.AspNetCore.Identity;

namespace NaturalAndNutritious.Tests
{
    public class UpdateResult : IdentityResult
    {
        public bool Succeeded { get; set; }
    }
}