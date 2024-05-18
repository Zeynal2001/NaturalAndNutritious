using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NaturalAndNutritious.Business.Abstractions;
using NaturalAndNutritious.Business.Abstractions.AdminPanelAbstractions;
using NaturalAndNutritious.Business.Dtos;
using NaturalAndNutritious.Business.Dtos.AdminPanelDtos;
using NaturalAndNutritious.Business.Extensions;
using NaturalAndNutritious.Data.Enums;
using NaturalAndNutritious.Presentation.Areas.admin_panel.Models;
using System.Security.Claims;

namespace NaturalAndNutritious.Presentation.Areas.admin_panel.Controllers
{
    [Area("admin_panel")]
    [Authorize(Roles = nameof(RoleTypes.Admin), AuthenticationSchemes = "AdminAuth")]
    public class UsersController : Controller
    {
        public UsersController(IUsersService usersService, IAuthService authService)
        {
            _usersService = usersService;
            _authService = authService;
        }

        private readonly IUsersService _usersService;
        private readonly IAuthService _authService;

        [Area("admin_panel")]
        [Authorize(Roles = nameof(RoleTypes.Admin), AuthenticationSchemes = "AdminAuth")]
        public async Task<IActionResult> GetAllUsers(int page = 1, int pageSize = 5)
        {
            var users = await _usersService.GetAllUsersWithPaginate(page, pageSize);

            var totalUsers = await _usersService.TotalUers();

            var vm = new GetAllUsersVm()
            {
                Users = users,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalUsers / (double)pageSize),
                PageSize = pageSize
            };
            return View(vm);
        }

        public async Task<IActionResult> Edit(string userId)
        {
            var user = await _usersService.GetUserByIdAsync(userId);

            if (user == null)
            {
                var errorModel = new ErrorModel()
                {
                    ErrorMessage = "There isn't such user"
                };

                return RedirectToAction("Error", errorModel);
            }

            var userEditDetails = new EditUserDto()
            {
                Id = user.Id,
                FirstName = user.FName,
                LastName = user.LName,
                NickName = user.UserName,
                BirthDate = user.BirthDate,
                Email = user.Email,
                ProfilePhotoUrl = user.ProfilePhotoUrl,
            };

            return View(userEditDetails);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _usersService.GetUserByIdAsync(model.Id);

            if (user == null)
            {
                ModelState.AddModelError("editError", "There isn't such user");
                return View(model);
            }

            var profilePhotoUrl = await _usersService.CompleteFileOperations(model);

            var result = await _usersService.EditUserDetails(user, profilePhotoUrl, model);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("editErrors", result.Message);
                ViewData["hasError"] = true;
                return View(model);
            }
            if (result.IsNull)
            {
                ModelState.AddModelError("editErrors", result.Message);
                ViewData["hasError"] = true;
                return View(model);
            }

            return RedirectToAction(nameof(GetAllUsers));
        }


        public async Task<IActionResult> ChangeRole(string Id)
        {
            var user = await _usersService.GetUserByIdAsync(Id);

            if (user == null)
            {
                var errorModel = new ErrorModel()
                {
                    ErrorMessage = "There isn't such user"
                };

                return View("Error", errorModel);
            }

            var currentRoles = await _usersService.GetUserRolesAsync(user);
            var roles = await _usersService.GetAllRoles();

            var roleVm = new ChangeRoleVm()
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.FName + " " + user.LName,
                ProfilePhotoPath = user.ProfilePhotoUrl,
                UserRoles = currentRoles.ToList(),
                Roles = roles
            };

            return View(roleVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeRole(ChangeRoleVm model)
        {
            var user = await _usersService.GetUserByIdAsync(model.Id);

            if (user == null)
            {
                var errorModel = new ErrorModel()
                {
                    ErrorMessage = "There isn't such user"
                };

                return View("Error", errorModel);
            }

            var role = await _usersService.GetRoleById(model.SelectedRoleId);

            if (role == null)
            {
                var errorModel = new ErrorModel()
                {
                    ErrorMessage = "There isn't such role."
                };

                return View("Error", errorModel);
            }

            if (await _usersService.IsInRoleAsync(user, role.Name))
            {
                var errorModel = new ErrorModel()
                {
                    ErrorMessage = "The user is already has this role."
                };

                return View("Error", errorModel);
            }

            var result = await _usersService.AddToRoleAsync(user, role.Name);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("roleError", result.Errors.ErrorsToString());
                return View(model);
            }

            return RedirectToAction(nameof(GetAllUsers));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string userId)
        {
            var currentUserId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (currentUserId == null)
            {
                var errorModel = new ErrorModel();
                errorModel.ErrorMessage = "You are not logged in as an admin!";

                return View("Error", errorModel);
            }

            var result = await _usersService.DeleteUser(currentUserId, userId);

            if (result.IsNull)
            {
                var errorModel = new ErrorModel();
                errorModel.ErrorMessage = result.Message;

                return View("Error", errorModel);
            }

            if (!result.IsDeleted)
            {
                var errorModel = new ErrorModel();
                errorModel.ErrorMessage = result.Message;
                return View("Error", errorModel);
            }

            return RedirectToAction(nameof(GetAllUsers));
        }

        public IActionResult Create()
        {
            ViewData["hasError"] = false;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RegisterDto model)
        {
            ViewData["hasError"] = false;

            if (!ModelState.IsValid)
            {
                ViewData["hasError"] = true;
                return View(model);
            }

            var result = await _authService.Register(model, "profile-photos");

            if (!result.Succeeded)
            {
                ViewData["hasError"] = true;
                ModelState.AddModelError("registerErrors", result.Message);
                return View(model);
            }

            if (result.Succeeded)
            {
                TempData["successMsg"] = result.Message;
                return RedirectToAction(nameof(GetAllUsers));
            }

            return View(model);
        }

        public IActionResult GetBannedUsers()
        {
            return View();
        }
    }
}
