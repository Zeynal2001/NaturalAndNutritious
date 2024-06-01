using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NaturalAndNutritious.Business.Abstractions;
using NaturalAndNutritious.Business.Dtos;
using NaturalAndNutritious.Data.Enums;
using NaturalAndNutritious.Presentation.ViewModels;
using System.Security.Claims;

namespace NaturalAndNutritious.Presentation.Controllers
{
    [Authorize(Roles = nameof(RoleTypes.Client))]
    public class ProfileController : Controller
    {
        public ProfileController(IProfileService profileSerivice, ILogger<ProfileController> logger)
        {
            _profileService = profileSerivice;
            _logger = logger;
        }

        private readonly IProfileService _profileService;
        private readonly ILogger<ProfileController> _logger;

        public async Task<IActionResult> Index()
        {
            try
            {
                ViewData["title"] = "Profile";

                var id = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

                if (id == null)
                {
                    ViewData["msg"] = "You aren't logged in";
                    _logger.LogError("User is not logged in");
                    return View("Error");
                }

                var userResult = await _profileService.GetUserByIdAsync(id.Value);
                if (userResult.IsNull)
                {
                    ViewData["msg"] = userResult.Message;
                    _logger.LogError($"Failed to get user by ID: {id.Value}. Reason: {userResult.Message}");
                    return View("Error");
                }

                var user = userResult.FoundUser;

                var models = await _profileService.GetModelsForView(user);
                if (models.IsNull)
                {
                    ViewData["msg"] = userResult.Message;
                    _logger.LogError($"Failed to get models for user {user.UserName}. Reason: {userResult.Message}");
                    return View("Error");
                }

                return View(models.ProfileIndexModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while processing the Profile/Index action: {ex.Message}");
                return View("Error");
            }
        }

        public async Task<IActionResult> Edit()
        {
            try
            {
                ViewData["title"] = "Edit";

                var Id = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                if (Id == null)
                {
                    ViewData["msg"] = "You aren't logged in";
                    _logger.LogError("User is not logged in");
                    return View("Error");
                }

                var userResult = await _profileService.GetUserByIdAsync(Id);
                if (userResult.IsNull)
                {
                    ViewData["msg"] = userResult.Message;
                    _logger.LogError($"Failed to get user by ID: {Id}. Reason: {userResult.Message}");
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
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while processing the Profile/Edit action: {ex.Message}");
                return View("Error");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProfileEditVm vm)
        {
            try
            {
                ViewData["hasError"] = false;

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("ModelState is invalid");
                    ViewData["hasError"] = true;
                    return View(vm);
                }

                var userResult = await _profileService.GetUserByIdAsync(vm.ProfileDetails.Id);

                if (userResult.IsNull)
                {
                    ViewData["msg"] = userResult.Message;
                    _logger.LogError($"User not found while editing profile. User ID: {vm.ProfileDetails.Id}");
                    return View("Error");
                }

                var user = userResult.FoundUser;

                var profilePhotoUrl = await _profileService.CompleteFileOperations(vm.ProfileDetails);

                var result = await _profileService.EditUserDetails(user, profilePhotoUrl, vm.ProfileDetails);

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("editErrors", result.Message);
                    ViewData["hasError"] = true;
                    _logger.LogError($"Failed to edit user details. User ID: {vm.ProfileDetails.Id}. Reason: {result.Message}");
                    return View(vm);
                }

                if (result.IsNull)
                {
                    ModelState.AddModelError("editErrors", result.Message);
                    ViewData["hasError"] = true;
                    _logger.LogError($"Edit operation returned null result. User ID: {vm.ProfileDetails.Id}");
                    return View(vm);
                }

                return View(vm);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while processing the Profile/Edit [HttpPost] action: {ex.Message}");
                return View("Error");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ProfileEditVm model)
        {
            try
            {
                ViewData["hasError"] = false;

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("ModelState is invalid");
                    ViewData["hasError"] = true;
                    return View(model);
                }

                var userResult = await _profileService.GetUserByIdAsync(model.ChangeDetails.Id);

                if (userResult.IsNull)
                {
                    ViewData["msg"] = userResult.Message;
                    _logger.LogError($"User not found while changing password. User ID: {model.ChangeDetails.Id}");
                    return View("Error");
                }

                var user = userResult.FoundUser;

                var result = await _profileService.ChangeUserPasswordResultAsync(user, model.ChangeDetails.CurrentPassword, model.ChangeDetails.NewPassword);
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
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while processing the Profile/ChangePassword [HttpPost] action: {ex.Message}");
                return View("Error");
            }
        }
    }
}
