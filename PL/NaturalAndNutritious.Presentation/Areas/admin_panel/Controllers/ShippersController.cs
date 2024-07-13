using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NaturalAndNutritious.Business.Dtos.AdminPanelDtos;
using NaturalAndNutritious.Business.Repositories;
using NaturalAndNutritious.Data.Abstractions;
using NaturalAndNutritious.Data.Entities;
using NaturalAndNutritious.Data.Enums;
using NaturalAndNutritious.Presentation.Areas.admin_panel.Models;

namespace NaturalAndNutritious.Presentation.Areas.admin_panel.Controllers
{
    [Area("admin_panel")]
    [Authorize(Roles = nameof(RoleTypes.Admin), AuthenticationSchemes = "AdminAuth")]
    public class ShippersController : Controller
    {
        private readonly IShipperRepository _shipperRepository;
        private readonly ILogger<ShippersController> _logger;

        public ShippersController(IShipperRepository shipperRepository, ILogger<ShippersController> logger)
        {
            _shipperRepository = shipperRepository;
            _logger = logger;
        }

        public async Task<IActionResult> GetAllShippers(int page = 1, int pageSize = 5)
        {
            _logger.LogInformation("GetAllShippers action called with page: {Page} and pageSize: {PageSize}", page, pageSize);

            var suppliersQueryable = await _shipperRepository.FilterWithPagination(page, pageSize);

            var shippers = await suppliersQueryable
                .OrderByDescending(s => s.CreatedAt)
                .Select(sc => new AllShippersDto()
                {
                    Id = sc.Id,
                    CompanyName = sc.CompanyName,
                    PhoneNumber = sc.PhoneNumber,
                }).ToListAsync();

            var totalShippers = await _shipperRepository.Table
                                         .OrderByDescending(o => o.CreatedAt)
                                         .CountAsync();

            var vm = new GetAllShippersVm()
            {
                Shippers = shippers,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalShippers / (double)pageSize),
                PageSize = pageSize
            };

            _logger.LogInformation("Retrieved {TotalShippers} suppliers.", totalShippers);

            return View(vm);
        }

        public IActionResult Create()
        {
            _logger.LogInformation("Create shipper GET action called.");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateShipperDto model)
        {
            _logger.LogInformation("Create shipper POST action called.");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState is invalid.");
                return View(model);
            }

            var shipper = new Shipper()
            {
                Id = Guid.NewGuid(),
                CompanyName = model.CompanyName,
                PhoneNumber = model.PhoneNumber,
                CreatedAt = DateTime.UtcNow,
            };

            await _shipperRepository.CreateAsync(shipper);
            int affected = await _shipperRepository.SaveChangesAsync();

            if (affected == 0)
            {
                var errorMessage = new ErrorModel { ErrorMessage = "The shipper could not be created." };
                return View("AdminError", errorMessage);
            }

            _logger.LogInformation("Shipper created successfully.");
            return RedirectToAction(nameof(GetAllShippers));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid Id)
        {
            _logger.LogInformation("Shipper Delete POST action called with shipperId: {Id}", Id);

            var isDeleted = await _shipperRepository.DeleteAsync(Id);
            await _shipperRepository.SaveChangesAsync();

            if (!isDeleted)
            {
                var errorModel = new ErrorModel { ErrorMessage = "There isn't such shipper." };
                _logger.LogWarning("Shipper with ID {Id} not found for deletion.", Id);
                return View("AdminError", errorModel);
            }

            _logger.LogInformation("Shipper deleted successfully for shipperId: {Id}", Id);
            return RedirectToAction(nameof(GetAllShippers));
        }

        [HttpPost]
        public async Task<IActionResult> AssumingDeleted(Guid Id)
        {
            _logger.LogInformation("Shipper AssumingDeleted POST action called with shipperId: {Id}", Id);

            var supplier = await _shipperRepository.GetByIdAsync(Id);

            if (supplier == null)
            {
                var errorModel = new ErrorModel { ErrorMessage = "There isn't such shipper." };
                _logger.LogWarning("Shipper with ID {Id} not found.", Id);
                return View("AdminError", errorModel);
            }

            supplier.IsDeleted = true;

            var affected = await _shipperRepository.SaveChangesAsync();

            if (affected == 0)
            {
                var errorMessage = new ErrorModel { ErrorMessage = "The shipper could not be created." };
                _logger.LogWarning("Shipper update to deleted failed for shipperId: {Id}", Id);
                return View("AdminError", errorMessage);
            }

            _logger.LogInformation("Shipper marked as deleted successfully for shipperId: {Id}", Id);
            return RedirectToAction(nameof(GetAllShippers));
        }
    }
}