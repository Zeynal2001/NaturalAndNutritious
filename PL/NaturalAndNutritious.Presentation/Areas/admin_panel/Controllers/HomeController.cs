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
            _logger.LogInformation("Admin panel HomeController Index action called");

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
            _logger.LogInformation("SendMailToUsers method called with Model: {Model}", model);

            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("SendMailToUsers method called with invalid model state. Error count: {ErrorCount}", ModelState.ErrorCount);
                    ViewData["hassError"] = ModelState.ErrorCount;
                    return View("Index", model);
                }

                var dto = new MailDto()
                {
                    Addresses = new List<MailboxAddress>()
                    {
                        new MailboxAddress("receiver", model.Recipient)
                    },
                    Subject = model.Subject,
                    Content = model.Message
                };

                await _emailService.SendAsync(dto);

                _logger.LogInformation("Email sent to user: {Recipient}, Subject: {Subject}", model.Recipient, model.Subject);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while sending email to users: {Recipient}, Subject: {Subject}", model.Recipient, model.Subject);

                var errorMessage = new ErrorModel { ErrorMessage = "An error occurred while sending email to users." };
                return View("AdminError", errorMessage);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMailToSubscribers(EmailModel model)
        {
            _logger.LogInformation("SendMailToSubscribers method called with Model: {Model}", model);

            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("SendMailToSubscribers method called with invalid model state. Error count: {ErrorCount}", ModelState.ErrorCount);
                    ViewData["hassError"] = ModelState.ErrorCount;
                    return View("Index", model);
                }

                var dto = new MailDto()
                {
                    Addresses = new List<MailboxAddress>()
                    {
                        new MailboxAddress("receiver", model.Recipient)
                    },
                    Subject = model.Subject,
                    Content = model.Message
                };

                await _emailService.SendAsync(dto);

                _logger.LogInformation("Email sent to subscribers: {Recipient}, Subject: {Subject}", model.Recipient, model.Subject);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                var msg = string.Format("An error occurred while sending email to subscribers. Email {0}, subject '{1}'", model.Recipient, model.Subject);
                _logger.LogError(ex, msg);

                //var errorMessage = new ErrorModel { ErrorMessage = msg };
                var errorMessage = new ErrorModel { ErrorMessage = "An error occurred while sending email to subscribers." };
                return View("AdminError", errorMessage);
            }
        }
    }
}