﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NaturalAndNutritious.Business.Abstractions;
using NaturalAndNutritious.Business.Abstractions.AdminPanelAbstractions;
using NaturalAndNutritious.Business.Dtos;

namespace NaturalAndNutritious.Presentation.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        public CheckoutController(IProductService productService, ILogger<CheckoutController> logger, IOrderService orderService)
        {
            _productService = productService;
            _logger = logger;
            _orderService = orderService;
        }

        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        private readonly ILogger<CheckoutController> _logger;

        public IActionResult Index()
        {
            _logger.LogInformation("Checkout process started. Displaying checkout page.");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(CheckoutDto model)
        {
            if (!ModelState.IsValid) 
            {
                _logger.LogWarning("Model state is invalid for model: {@model}", model);
                return View(model);
            }

            _logger.LogInformation("Checkout process started with the following data: {CheckoutDto}", model);

            var (success, message) = await _orderService.ProcessOrderAsync(model, User, HttpContext.Session);

            if (success)
            {
                TempData["successMsg"] = message;
                _logger.LogInformation("Order processed successfully: {Message}", message);
                return RedirectToAction(nameof(OrderResult));
            }

            _logger.LogError("Error processing order: {ErrorMessage}", message);
            ViewData["msg"] = message;
            return View("Error");
        }

        public IActionResult OrderResult()
        {
            _logger.LogInformation("Order result page accessed.");

            var successMsg = TempData["successMsg"] as string;
            ViewData["successMsg"] = successMsg;

            return View();
        }
    }
}