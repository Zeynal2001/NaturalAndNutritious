using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using NaturalAndNutritious.Business.Abstractions;
using NaturalAndNutritious.Business.Dtos;
using NaturalAndNutritious.Data.Abstractions;
using NaturalAndNutritious.Data.Enums;
using NaturalAndNutritious.Presentation.Areas.admin_panel.Models;

namespace NaturalAndNutritious.Presentation.Areas.admin_panel.Controllers
{
    [Area("admin_panel")]
    [Authorize(Roles = nameof(RoleTypes.Admin), AuthenticationSchemes = "AdminAuth")]
    public class HomeController : Controller
    {
        public HomeController(IProductRepository productRepository, IUserRepository userRepository, ILogger<HomeController> logger, IEmailService emailService, IOrderRepository orderRepository)
        {
            _productRepository = productRepository;
            _userRepository = userRepository;
            _logger = logger;
            _emailService = emailService;
            _orderRepository = orderRepository;
        }

        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IEmailService _emailService;
        private readonly ILogger<HomeController> _logger;

        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("AdminHomeIndex action called");

            var products = await _productRepository.GetAllAsync();
            _logger.LogInformation("Fetched {ProductCount} products from the database", products.Count());

            var orders = await _orderRepository.GetAllAsync();
            _logger.LogInformation("Fetched {OrderCount} orders from the database", orders.Count());

            var users = await _userRepository.GetAllUsers();
            _logger.LogInformation("Fetched {UserCount} users from the database", users.Count());

            ViewBag.totalUsers = users.Count();
            ViewBag.totalProducts = products.Count();
            ViewBag.totalViews = products.Sum(p => p.ViewsCount);
            ViewBag.totalBudget = products.Sum(p => p.ProductPrice);
            ViewBag.totalRevenue = orders.Sum(o => o.Freight);

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMailToUsers(EmailModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["hassError"] = ModelState.ErrorCount;
                return View("Index", model);
            }

            var dto = new MailDto()
            {
                 Addresses = new List<MailboxAddress>()
                 {
                     new MailboxAddress("reciever", model.Recipient)
                 },
                 Subject = model.Subject,
                Content = model.Message
            };

            await _emailService.SendAsync(dto);

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMailToSubscribers(EmailModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["hassError"] = ModelState.ErrorCount;
                return View("Index", model);
            }

            var dto = new MailDto()
            {
                Addresses = new List<MailboxAddress>()
                 {
                     new MailboxAddress("reciever", model.Recipient)
                 },
                Subject = model.Subject,
                Content = model.Message
            };

            await _emailService.SendAsync(dto);

            return RedirectToAction("Index");
        }
    }
}
