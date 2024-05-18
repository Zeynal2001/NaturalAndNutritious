using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NaturalAndNutritious.Data.Enums;

namespace NaturalAndNutritious.Presentation.Areas.admin_panel.Controllers
{
    public class HomeController : Controller
    {
        [Area("admin_panel")]
        [Authorize(Roles = nameof(RoleTypes.Admin), AuthenticationSchemes = "AdminAuth")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
