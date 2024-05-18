using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NaturalAndNutritious.Business.Abstractions;
using NaturalAndNutritious.Business.Dtos;
using NaturalAndNutritious.Data.Enums;
using NaturalAndNutritious.Presentation.ViewModels;
using SessionMapper;
using System.Security.Claims;

namespace NaturalAndNutritious.Presentation.Controllers
{
    [Authorize(Roles = nameof(RoleTypes.Client))]
    public class ProfileController : Controller
    {
        public ProfileController(IProfileService profileSerivice)
        {
            _profileService = profileSerivice;
        }

        private readonly IProfileService _profileService;

        public async Task<IActionResult> Index()
        {
            ViewData["title"] = "Profile";

            var id = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            if (id == null)
            {
                ViewData["msg"] = "You aren't logged in";
                return View("Error");
            }

            var userResult = await _profileService.GetUserByIdAsync(id.Value);
            if (userResult.IsNull)
            {
                ViewData["msg"] = userResult.Message;
                return View("Error");
            }

            var user = userResult.FoundUser;

            var models = await _profileService.GetModelsForView(user);
            if (models.IsNull)
            {
                ViewData["msg"] = userResult.Message;
                return View("Error");
            }

            return View(models.ProfileIndexModel);
        }


        public async Task<IActionResult> Edit()
        {
            ViewData["title"] = "Edit";

            var Id = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (Id == null)
            {
                ViewData["msg"] = "You aren't logged in";
                return View("Error");
            }

            var userResult = await _profileService.GetUserByIdAsync(Id);
            if (userResult.IsNull)
            {
                ViewData["msg"] = userResult.Message;
                return View("Error");
            }

            var user = userResult.FoundUser;

            var profileEditDetails = new ProfileEditDto()
            {
                Id = user.Id,
                FirstName = user.FName,
                LastName = user.LName,
                Email = user.Email,
                NickName = user.UserName,
                ProfilePhotoUrl = user.ProfilePhotoUrl,
                BirthDate = user.BirthDate
            };

            var editVm = new ProfileEditVm()
            {
                ProfileDetails = profileEditDetails
            };

            ViewData["hasError"] = false;

            return View(editVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProfileEditVm vm)
        {
            ViewData["hasError"] = false;

            if (!ModelState.IsValid)
            {
                ViewData["hasError"] = true;
                return View(vm);
            }

            var userResult = await _profileService.GetUserByIdAsync(vm.ProfileDetails.Id);

            if (userResult.IsNull)
            {
                ViewData["msg"] = userResult.Message;
                return View("Error");
            }

            var user = userResult.FoundUser;

            var profilePhotoUrl = await _profileService.CompleteFileOperations(vm.ProfileDetails);

            var result = await _profileService.EditUserDetails(user, profilePhotoUrl, vm.ProfileDetails);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("editErrors", result.Message);
                ViewData["hasError"] = true;
                return View(vm);
            }
            if (result.IsNull)
            {
                ModelState.AddModelError("editErrors", result.Message);
                ViewData["hasError"] = true;
                return View(vm);
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ProfileEditVm model)
        {
            ViewData["hasError"] = false;

            if (!ModelState.IsValid)
            {
                ViewData["hasError"] = true;
                return View(model);
            }

            var userResult = await _profileService.GetUserByIdAsync(model.ChangeDetails.Id);

            if (userResult.IsNull)
            {
                ViewData["msg"] = userResult.Message;
                return View("Error");
            }

            var user = userResult.FoundUser;

            var result = await _profileService.ChangeUserPasswordAsync(user, model.ChangeDetails.CurrentPassword, model.ChangeDetails.NewPassword);
            if (result.Succeeded)
            {
                TempData["succesMsg"] = result.Message;
                return RedirectToAction("Index", "Profile");
            }

            ModelState.AddModelError("editErrors", result.Message);
            ViewData["hasError"] = true;
            TempData["errorMsg"] = result.Message;

            return RedirectToAction("Edit");
        }
    }
}
