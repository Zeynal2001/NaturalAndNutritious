using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using NaturalAndNutritious.Business.Abstractions;
using NaturalAndNutritious.Business.Dtos;
using NaturalAndNutritious.Business.Dtos.AdminPanelDtos;
using NaturalAndNutritious.Data.Abstractions;
using NaturalAndNutritious.Data.Enums;
using NaturalAndNutritious.Presentation.Areas.admin_panel.Models;

namespace NaturalAndNutritious.Presentation.Areas.admin_panel.Controllers
{
    [Area("admin_panel")]
    [Authorize(Roles = nameof(RoleTypes.Admin), AuthenticationSchemes = "AdminAuth")]
    public class ContactMessagesController : Controller
    {
        private readonly ILogger<ContactMessagesController> _logger;
        private readonly IMessageRepository _messageRepository;
        private readonly IEmailService _emailService;

        public ContactMessagesController(ILogger<ContactMessagesController> logger, IMessageRepository messageRepository, IEmailService emailService)
        {
            _logger = logger;
            _messageRepository = messageRepository;
            _emailService = emailService;
        }

        public async Task<IActionResult> GetAllMessages(int page = 1, int pageSize = 5)
        {
            _logger.LogInformation("GetAllMessages action called with page: {Page} and pageSize: {PageSize}", page, pageSize);

            var messagesQueryable = await _messageRepository.FilterWithPagination(page, pageSize);
            var messages = await messagesQueryable
                .OrderByDescending(sc => sc.CreatedAt)
                .Select(m => new AllMessagesDto()
                {
                    Id = m.Id,
                    CustomerName = m.CustomerName,
                    CustomerEmailAddress = m.CustomerEmailAddress,
                    IsAnswered = m.IsAnswered,
                    CreatedAt = m.CreatedAt,
                    UpdatedAt = m.UpdatedAt,
                    IsDeleted = m.IsDeleted
                }).ToListAsync();

            var totalMessages = await _messageRepository.Table
                        .Where(m => !m.IsDeleted)
                        .OrderByDescending(sc => sc.CreatedAt)
                        .CountAsync();

            var vm = new GetAllMessagesVm()
            {
                Messages = messages,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalMessages / (double)pageSize),
                PageSize = pageSize
            };

            _logger.LogInformation("Retrieved {TotalMessages} messages.", totalMessages);

            return View(vm);
        }

        public async Task<IActionResult> SeeMessage(Guid Id)
        {
            _logger.LogInformation("SeeMessage action called");

            var message = await _messageRepository.GetByIdAsync(Id);

            if (message == null)
            {
                _logger.LogWarning("Requested message with ID {MessageId} not found", Id);
                ViewData["msg"] = "There isn't such message";
                return View("Error");
            }

            var model = new SeeMessageModel()
            {
                Id = message.Id,
                Name = message.CustomerName,
                EmailAddress = message.CustomerEmailAddress,
                MessageText = message.CustomerMessage,
                MessageDate = message.CreatedAt
            };

            return View(model);
        }

        public async Task<IActionResult> ReplyToMessage(Guid Id)
        {
            _logger.LogInformation("ReplyToMessage GET action called");

            var message = await _messageRepository.GetByIdAsync(Id);

            if (message == null)
            {
                _logger.LogWarning("Requested message with ID {MessageId} not found", Id);
                ViewData["msg"] = "There isn't such message";
                return View("Error");
            }

            if (message.IsAnswered)
            {
                var errorModel = new ErrorModel
                {
                    ErrorMessage = "The message has already been answered."
                };

                return View("AdminError", errorModel);
            }

            var model = new ReplyModel
            {
                Id = message.Id,
                Recipient = message.CustomerEmailAddress
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReplyToMessage(ReplyModel model)
        {
            _logger.LogInformation("ReplyToMessage POST action called");

            if (!ModelState.IsValid)
            {
                ViewData["hasError"] = ModelState.ErrorCount;
                _logger.LogWarning("ModelState is invalid with {ErrorCount} errors", ModelState.ErrorCount);
                return View("Index", model);
            }

            var dto = new MailDto
            {
                Addresses = new List<MailboxAddress>
                {
                    new MailboxAddress("reciever", model.Recipient)
                },
                Subject = model.Subject,
                Content = model.Message
            };

            try
            {
                await _emailService.SendAsync(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Recipient}", model.Recipient);
                var errorModel = new ErrorModel
                {
                    ErrorMessage = "Failed to send email."
                };
                return View("AdminError", errorModel);
            }

            var message = await _messageRepository.GetByIdAsync(model.Id);

            if (message == null)
            {
                var errorModel = new ErrorModel
                {
                    ErrorMessage = "There isn't such message."
                };

                _logger.LogWarning("Message not found for messageId: {MessageId}", model.Id);
                return View("AdminError", errorModel);
            }

            message.IsAnswered = true;
            message.UpdatedAt = DateTime.UtcNow;

            try
            {
                var isUpdated = await _messageRepository.UpdateAsync(message);
                await _messageRepository.SaveChangesAsync();

                if (!isUpdated)
                {
                    var errorModel = new ErrorModel
                    {
                        ErrorMessage = "Message update failed"
                    };

                    _logger.LogError("Failed to update message with ID {MessageId}", model.Id);
                    return View("AdminError", errorModel);
                }

                _logger.LogInformation("Successfully replied to message with ID {MessageId}", model.Id);
                return RedirectToAction(nameof(GetAllMessages));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the message with messageId: {messageId}", message.Id);
                var errorMessage = new ErrorModel { ErrorMessage = "An error occurred while updating the message." };
                return View("AdminError", errorMessage);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid Id)
        {
            _logger.LogInformation("Delete POST action called with messageId: {Id}", Id);

            var isDeleted = await _messageRepository.DeleteAsync(Id);
            await _messageRepository.SaveChangesAsync();

            if (!isDeleted)
            {
                var errorModel = new ErrorModel { ErrorMessage = "There isn't such shipper." };
                _logger.LogWarning("Message with ID {Id} not found for deletion.", Id);
                return View("AdminError", errorModel);
            }

            _logger.LogInformation("Message deleted successfully for messageId: {Id}", Id);
            return RedirectToAction(nameof(GetAllMessages));
        }

        [HttpPost]
        public async Task<IActionResult> AssumingDeleted(Guid Id)
        {
            _logger.LogInformation("AssumingDeleted POST action called with messageId: {Id}", Id);

            var supplier = await _messageRepository.GetByIdAsync(Id);

            if (supplier == null)
            {
                var errorModel = new ErrorModel { ErrorMessage = "There isn't such message." };
                _logger.LogWarning("Message with ID {Id} not found.", Id);
                return View("AdminError", errorModel);
            }
            supplier.IsDeleted = true;

            var affected = await _messageRepository.SaveChangesAsync();

            if (affected == 0)
            {
                var errorMessage = new ErrorModel { ErrorMessage = "The message could not be created." };
                _logger.LogWarning("Message update to deleted failed for messageId: {Id}", Id);
                return View("AdminError", errorMessage);
            }

            _logger.LogInformation("Message marked as deleted successfully for messageId: {Id}", Id);
            return RedirectToAction(nameof(GetAllMessages));
        }
    }
}