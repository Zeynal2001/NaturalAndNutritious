using Microsoft.AspNetCore.Mvc;
using NaturalAndNutritious.Business.Dtos;
using NaturalAndNutritious.Data.Abstractions;
using NaturalAndNutritious.Data.Entities;

namespace NaturalAndNutritious.Presentation.Controllers
{
    public class SubscribersController : Controller
    {
        private readonly ISubscriberRepository _subscriberRepository;
        private readonly ILogger<SubscribersController> _logger;

        public SubscribersController(ISubscriberRepository subscriberRepository, ILogger<SubscribersController> logger)
        {
            _subscriberRepository = subscriberRepository;
            _logger = logger;
        }

        //public IActionResult AddSubscriber()
        //{
        //    return View();
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSubscriber(SubscriberDto model)
        {
            _logger.LogInformation("AddSubscriber POST action called");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState is invalid");
                ViewData["hassErrors"] = ModelState.ErrorCount;
                return View(model);
            }

            var affected = 0;
            try
            {
                var subscriber = new Subscriber()
                {
                    Id = Guid.NewGuid(),
                    Email = model.SubscriberEmail,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                };
                _logger.LogInformation("Creating subscriber with email: {Email}", subscriber.Email);
                await _subscriberRepository.CreateAsync(subscriber);
                affected = await _subscriberRepository.SaveChangesAsync();

                if (affected == 0)
                {
                    _logger.LogWarning("Subscriber creation failed. Affected rows: {Rows}", affected);
                    ViewData["msg"] = "Subscriber creation failed";
                    return View("Error");
                }

                _logger.LogInformation("Subscriber created successfully with email: {Email}", subscriber.Email);
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while adding subscriber with email {Email}: {Exception}", model.SubscriberEmail, ex.ToString());
                return View("Error");
            }
        }
    }
}
