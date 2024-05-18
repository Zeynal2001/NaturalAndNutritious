using Microsoft.AspNetCore.Identity;

namespace NaturalAndNutritious.Presentation.Areas.admin_panel.Models
{
    public class ChangeRoleVm
    {
        public string Id { get; set; }
        public string? UserName { get; set; }
        public string? ProfilePhotoPath { get; set; }
        public string? Email { get; set; }
        public List<string> UserRoles { get; set; }
        public List<IdentityRole> Roles { get; set; }
        public string SelectedRoleId { get; set; }
    }
}
