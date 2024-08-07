﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using NaturalAndNutritious.Business.Abstractions;
using NaturalAndNutritious.Business.Dtos;
using NaturalAndNutritious.Business.Dtos.AdminPanelDtos;
using NaturalAndNutritious.Data.Abstractions;
using NaturalAndNutritious.Data.Entities;
using NaturalAndNutritious.Data.Enums;
using NaturalAndNutritious.Presentation.Areas.admin_panel.Models;

namespace NaturalAndNutritious.Presentation.Areas.admin_panel.Controllers
{
    [Area("admin_panel")]
    [Authorize(Roles = nameof(RoleTypes.Admin), AuthenticationSchemes = "AdminAuth")]
    public class SubscribersController : Controller
    {
        private readonly IEmailService _emailService;
        private readonly ISubscriberRepository _subscriberRepository;
        private readonly ILogger<SubscribersController> _logger;

        public SubscribersController(IEmailService emailService, ISubscriberRepository subscriberRepository, ILogger<SubscribersController> logger)
        {
            _emailService = emailService;
            _subscriberRepository = subscriberRepository;
            _logger = logger;
        }

        public async Task<IActionResult> GetAllSubscribers(int page = 1, int pageSize = 5)
        {
            _logger.LogInformation("GetAllSubscribers action called with page: {Page} and pageSize: {PageSize}", page, pageSize);

            var subscribersQueryable = await _subscriberRepository.FilterWithPagination(page, pageSize);
            var subscribers = await subscribersQueryable
                .OrderByDescending(sb => sb.CreatedAt)
                .Select(sb => new AllSubscribersDto()
                {
                    Id = sb.Id,
                    SubscriberEmail = sb.Email,
                    CreatedAt = sb.CreatedAt,
                    IsDeleted = sb.IsDeleted,
                    UpdatedAt = sb.UpdatedAt
                }).ToListAsync();

            var totalSubscribers = await _subscriberRepository.Table
                        .Where(sb => !sb.IsDeleted)
                        .OrderByDescending(sb => sb.CreatedAt)
                        .CountAsync();

            var vm = new GetAllSubscribersVm()
            {
                Subscribers = subscribers,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalSubscribers / (double)pageSize),
                PageSize = pageSize
            };

            _logger.LogInformation("Retrieved {TotalSubscribers} subscribers.", totalSubscribers);

            return View(vm);
        }

        public IActionResult Create()
        {
            _logger.LogInformation("Subscriber Create GET method called.");
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Create(CreateSubscriberDto model)
        {
            _logger.LogInformation("Subscriber Create POST method called with Model: {Model}", model);

            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Create method called with invalid model state. Error count: {ErrorCount}", ModelState.ErrorCount);
                    return View(model);
                }

                int affected = 0;

                var subscriber = new Subscriber()
                {
                    Id = Guid.NewGuid(),
                    Email = model.SubscriberEmail,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow
                };

                await _subscriberRepository.CreateAsync(subscriber);
                affected = await _subscriberRepository.SaveChangesAsync();

                if (affected == 0)
                {
                    var errorMessage = new ErrorModel { ErrorMessage = "The subscriber could not be created." };
                    _logger.LogError("Subscriber creation failed. No rows affected.");
                    return View("AdminError", errorMessage);
                }

                _logger.LogInformation("Subscriber created successfully with ID: {SubscriberId}", subscriber.Id);
                return RedirectToAction(nameof(GetAllSubscribers));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the subscriber.");
                var errorModel = new ErrorModel { ErrorMessage = "An unexpected error occurred." };
                return View("AdminError", errorModel);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid Id)
        {
            _logger.LogInformation("Subscriber Delete action called with Id: {Id}", Id);

            try
            {
                var isDeleted = await _subscriberRepository.DeleteAsync(Id);
                await _subscriberRepository.SaveChangesAsync();

                if (!isDeleted)
                {
                    var errorModel = new ErrorModel
                    {
                        ErrorMessage = "There isn't such subscriber."
                    };

                    _logger.LogWarning("Subscriber deletion failed for Id: {Id}", Id);
                    return View("AdminError", errorModel);
                }

                _logger.LogInformation("Subscriber successfully deleted for Id: {Id}", Id);
                return RedirectToAction(nameof(GetAllSubscribers));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the subscriber with Id: {Id}", Id);
                var errorModel = new ErrorModel { ErrorMessage = "An unexpected error occurred." };
                return View("AdminError", errorModel);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AssumingDeleted(Guid Id)
        {
            _logger.LogInformation("Subscriber AssumingDeleted action called with Id: {Id}", Id);

            var subscriber = await _subscriberRepository.GetByIdAsync(Id);

            if (subscriber == null)
            {
                var errorModel = new ErrorModel
                {
                    ErrorMessage = "There isn't such subscriber."
                };

                _logger.LogWarning("Subscriber not found for Id: {Id}", Id);
                return View("AdminError", errorModel);
            }

            subscriber.IsDeleted = true;

            try
            {
                var isUpdated = await _subscriberRepository.UpdateAsync(subscriber);
                await _subscriberRepository.SaveChangesAsync();

                if (!isUpdated)
                {
                    var errorModel = new ErrorModel
                    {
                        ErrorMessage = "Subscriber not updated."
                    };

                    _logger.LogWarning("Subscriber update failed for Id: {Id}", Id);
                    return View("AdminError", errorModel);
                }

                _logger.LogInformation("Subscriber marked as deleted for Id: {Id}", Id);
                return RedirectToAction(nameof(GetAllSubscribers));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the subscriber with Id: {Id}", Id);
                var errorModel = new ErrorModel { ErrorMessage = "An unexpected error occurred." };
                return View("AdminError", errorModel);
            }
        }

        public async Task<IActionResult> SendBatchEmail()
        {
            _logger.LogInformation("SendBatchEmail GET method called.");

            try
            {
                var subscribersAsQueryable = await _subscriberRepository.GetAllAsync();
                var subscribers = subscribersAsQueryable.OrderBy(subs => subs.Email).ToList();

                ViewBag.Subscribers = subscribers;

                _logger.LogInformation("Subscribers retrieved successfully.");

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving subscribers.");
                var errorModel = new ErrorModel { ErrorMessage = "An unexpected error occurred." };
                return View("AdminError", errorModel);
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> SendBatchEmail(SendBatchEmailModel model)
        {
            _logger.LogInformation("SendBatchEmail POST method called with Model: {Model}", model);

            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("SendBatchEmail POST method called with invalid model state. Error count: {ErrorCount}", ModelState.ErrorCount);
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

                    _logger.LogInformation("Email sent to: {Email}, Subject: {Subject}", mail, model.Subject);
                }

                _logger.LogInformation("All emails sent successfully.");

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while sending batch emails.");
                var errorModel = new ErrorModel { ErrorMessage = "An unexpected error occurred." };
                return View("AdminError", errorModel);
            }
        }
    }
}