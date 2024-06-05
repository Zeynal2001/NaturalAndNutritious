using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using NaturalAndNutritious.Business.Abstractions;
using NaturalAndNutritious.Business.Abstractions.AdminPanelAbstractions;
using NaturalAndNutritious.Business.Dtos;
using NaturalAndNutritious.Business.Dtos.AdminPanelDtos;
using NaturalAndNutritious.Business.Extensions;
using NaturalAndNutritious.Data.Abstractions;
using NaturalAndNutritious.Data.Entities;
using NaturalAndNutritious.Data.Enums;
using NaturalAndNutritious.Presentation.Areas.admin_panel.Models;
using System.Security.Claims;

namespace NaturalAndNutritious.Presentation.Areas.admin_panel.Controllers
{
    [Area("admin_panel")]
    [Authorize(Roles = nameof(RoleTypes.Admin), AuthenticationSchemes = "AdminAuth")]
    public class UsersController : Controller
    {
        public UsersController(IUserService usersService, IAuthService authService, IUserRepository userRepository, UserManager<AppUser> userManager, ILogger<UsersController> logger, IEmailService emailService)
        {
            _usersService = usersService;
            _authService = authService;
            _userRepository = userRepository;
            _userManager = userManager;
            _emailService = emailService;
            _logger = logger;
        }

        private readonly IUserService _usersService;
        private readonly IAuthService _authService;
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<UsersController> _logger;

        [Area("admin_panel")]
        [Authorize(Roles = nameof(RoleTypes.Admin), AuthenticationSchemes = "AdminAuth")]
        public async Task<IActionResult> GetAllUsers(int page = 1, int pageSize = 5)
        {
            _logger.LogInformation("GetAllUsers action called with page: {Page} and pageSize: {PageSize}", page, pageSize);

            var users = await _usersService.GetAllUsersWithPaginate(page, pageSize);

            var totalUsers = await _usersService.TotalUers();

            var vm = new GetAllUsersVm()
            {
                Users = users,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalUsers / (double)pageSize),
                PageSize = pageSize
            };

            _logger.LogInformation("Retrieved {TotalUsers} users.", totalUsers);

            return View(vm);
        }

        public async Task<IActionResult> Edit(string userId)
        {
            _logger.LogInformation("Edit GET action called with userId: {UserId}", userId);

            var user = await _usersService.GetUserByIdAsync(userId);

            if (user == null)
            {
                var errorModel = new ErrorModel()
                {
                    ErrorMessage = "There isn't such user"
                };

                return RedirectToAction("AdminError", errorModel);
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
            _logger.LogInformation("Edit POST action called.");

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

            _logger.LogInformation("User details updated successfully for userId: {UserId}", model.Id);
            return RedirectToAction(nameof(GetAllUsers));
        }


        public async Task<IActionResult> ChangeRole(string Id)
        {
            _logger.LogInformation("ChangeRole GET action called with userId: {UserId}", Id);

            var user = await _usersService.GetUserByIdAsync(Id);

            if (user == null)
            {
                var errorModel = new ErrorModel()
                {
                    ErrorMessage = "There isn't such user"
                };

                return View("AdminError", errorModel);
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
            _logger.LogInformation("ChangeRole POST action called.");

            var user = await _usersService.GetUserByIdAsync(model.Id);

            if (user == null)
            {
                var errorModel = new ErrorModel()
                {
                    ErrorMessage = "There isn't such user"
                };

                return View("AdminError", errorModel);
            }

            var role = await _usersService.GetRoleById(model.SelectedRoleId);

            if (role == null)
            {
                var errorModel = new ErrorModel()
                {
                    ErrorMessage = "There isn't such role."
                };

                return View("AdminError", errorModel);
            }

            if (await _usersService.IsInRoleAsync(user, role.Name))
            {
                var errorModel = new ErrorModel()
                {
                    ErrorMessage = "The user already has this role."
                };

                return View("AdminError", errorModel);
            }

            var result = await _usersService.AddToRoleAsync(user, role.Name);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("roleError", result.Errors.ErrorsToString());
                return View(model);
            }

            _logger.LogInformation("Role changed successfully for userId: {UserId}", model.Id);
            return RedirectToAction(nameof(GetAllUsers));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string userId)
        {
            _logger.LogInformation("Delete POST action called with userId: {UserId}", userId);

            var currentUserId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (currentUserId == null)
            {
                var errorModel = new ErrorModel();
                errorModel.ErrorMessage = "You are not logged in as an admin!";

                return View("AdminError", errorModel);
            }

            var result = await _usersService.DeleteUser(currentUserId, userId);

            if (result.IsNull)
            {
                var errorModel = new ErrorModel();
                errorModel.ErrorMessage = result.Message;

                return View("AdminError", errorModel);
            }

            if (!result.IsDeleted)
            {
                var errorModel = new ErrorModel();
                errorModel.ErrorMessage = result.Message;

                return View("AdminError", errorModel);
            }

            _logger.LogInformation("User deleted successfully for userId: {UserId}", userId);
            return RedirectToAction(nameof(GetAllUsers));
        }

        public IActionResult Create()
        {
            _logger.LogInformation("Create GET action called.");

            ViewData["hasError"] = false;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RegisterDto model)
        {
            _logger.LogInformation("Create POST action called.");

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
                _logger.LogInformation("User created successfully with email: {Email}", model.Email);
                return RedirectToAction(nameof(GetAllUsers));
            }

            return View(model);
        }

        public IActionResult GetBannedUsers()
        {
            _logger.LogInformation("GetBannedUsers GET action called.");

            return View();
        }

        public async Task<IActionResult> SendBatchEmail()
        {
            var usersAsQueryable = await _userRepository.GetAllUsers();
            var users = usersAsQueryable.OrderBy(usr => usr.FName + " " + usr.FName).ToList();

            ViewBag.Users = users;

            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> SendBatchEmail(SendBatchEmailModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            foreach (var mail in model.Emails)
            {
                var dto = new MailDto()
                {
                    Addresses = new List<MailboxAddress>() { new MailboxAddress(mail.Split("@")[0], mail) },
                    Subject = model.Subject,
                    Content = model.Message
                };

                await _emailService.SendAsync(dto);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
