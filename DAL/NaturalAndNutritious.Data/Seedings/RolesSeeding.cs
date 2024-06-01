using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NaturalAndNutritious.Data.Enums;

namespace NaturalAndNutritious.Data.Seedings
{
    public static class RolesSeeding
    {
        public static void SeedRoles(this ModelBuilder modelBuilder)
        {
            var roles = Enum.GetNames(typeof(RoleTypes));

            var identityRoles = new List<IdentityRole>();

            foreach (var role in roles)
            {
                identityRoles.Add(new IdentityRole()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = role,
                    NormalizedName = role.ToUpper()
                });
                //identityRoles.Add(new IdentityRole(roleType));
            }

            modelBuilder.Entity<IdentityRole>()
                .HasData(identityRoles);
        }
    }
}
