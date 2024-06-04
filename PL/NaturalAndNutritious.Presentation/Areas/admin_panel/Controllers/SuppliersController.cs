using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NaturalAndNutritious.Business.Abstractions.AdminPanelAbstractions;
using NaturalAndNutritious.Business.Dtos.AdminPanelDtos;
using NaturalAndNutritious.Data.Abstractions;
using NaturalAndNutritious.Data.Enums;
using NaturalAndNutritious.Presentation.Areas.admin_panel.Models;

namespace NaturalAndNutritious.Presentation.Areas.admin_panel.Controllers
{
    [Area("admin_panel")]
    [Authorize(Roles = nameof(RoleTypes.Admin), AuthenticationSchemes = "AdminAuth")]
    public class SuppliersController : Controller
    {
        public SuppliersController(ISupplierRepository supplierRepository, ISupplierService supplierService, ILogger<SuppliersController> logger)
        {
            _supplierRepository = supplierRepository;
            _supplierService = supplierService;
            _logger = logger;
        }

        private readonly ISupplierRepository _supplierRepository;
        private readonly ISupplierService _supplierService;
        private readonly ILogger<SuppliersController> _logger;

        [Area("admin_panel")]
        [Authorize(Roles = nameof(RoleTypes.Admin), AuthenticationSchemes = "AdminAuth")]
        public async Task<IActionResult> GetAllSuppliers(int page = 1, int pageSize = 5)
        {
            _logger.LogInformation("GetAllSuppliers action called with page: {Page} and pageSize: {PageSize}", page, pageSize);

            var suppliersQueryable = await _supplierRepository.FilterWithPagination(page, pageSize);

            var suppliers = await suppliersQueryable
                .OrderByDescending(sc => sc.CreatedAt)
                .Select(sc => new AllSuppliersDto()
                {
                    Id = sc.Id.ToString(),
                    CompanyName = sc.CompanyName,
                    ContactName = sc.ContactName,
                    ContactTitle = sc.ContactTitle,
                    Address = sc.Address,
                    City = sc.City,
                    Region = sc.Region,
                    PostalCode = sc.PostalCode,
                    Country = sc.Country,
                    PhoneNumber = sc.PhoneNumber,
                    Fax = sc.Fax,
                    Website = sc.Website,
                }).ToListAsync();

            var totalSuppliers = await _supplierService.TotalSuppliers();

            var vm = new GetAllSuppliersVm()
            {
                Suppliers = suppliers,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalSuppliers / (double)pageSize),
                PageSize = pageSize
            };

            _logger.LogInformation("Retrieved {TotalSuppliers} suppliers.", totalSuppliers);

            return View(vm);
        }

        public IActionResult Create()
        {
            _logger.LogInformation("Create GET action called.");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateSupplierDto model)
        {
            _logger.LogInformation("Create POST action called.");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState is invalid.");
                return View(model);
            }

            var result = await _supplierService.CreateSupplier(model);

            if (!result.Succeeded)
            {
                _logger.LogWarning("Supplier creation failed: {Message}", result.Message);
                var error = new ErrorModel { ErrorMessage = result.Message };
                return View("AdminError", error);
            }

            _logger.LogInformation("Supplier created successfully.");
            return RedirectToAction(nameof(GetAllSuppliers));
        }

        public async Task<IActionResult> Update(string Id)
        {
            _logger.LogInformation("Update GET action called with supplierId: {Id}", Id);

            if (!Guid.TryParse(Id, out var guidId))
            {
                var errorMessage = $"The id '{Id}' is not a valid GUID.";
                _logger.LogError(errorMessage);
                throw new ArgumentException(errorMessage, nameof(Id));
            }

            var supplier = await _supplierRepository.GetByIdAsync(guidId);

            if (supplier == null)
            {
                var errorModel = new ErrorModel { ErrorMessage = "There isn't such supplier." };
                _logger.LogWarning("Supplier with ID {Id} not found.", Id);
                return View("AdminError", errorModel);
            }

            var supplierDetails = new UpdateSupplierDto()
            {
                Id = supplier.Id.ToString(),
                CompanyName = supplier.CompanyName,
                ContactName = supplier.ContactName,
                ContactTitle = supplier.ContactTitle,
                Address = supplier.Address,
                City = supplier.City,
                Country = supplier.Country,
                Region = supplier.Region,
                PostalCode = supplier.PostalCode,
                PhoneNumber = supplier.PhoneNumber,
                Fax = supplier.Fax,
                Website = supplier.Website,
            };

            _logger.LogInformation("Supplier details retrieved successfully for supplierId: {Id}", Id);
            ViewData["hasError"] = false;

            return View(supplierDetails);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UpdateSupplierDto model)
        {
            _logger.LogInformation("Update POST action called.");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState is invalid.");
                return View(model);
            }

            if (!Guid.TryParse(model.Id, out var guidId))
            {
                var errorMessage = $"The id '{model.Id}' is not a valid GUID.";
                _logger.LogError(errorMessage);
                throw new ArgumentException(errorMessage, nameof(model.Id));
            }

            var supplier = await _supplierRepository.GetByIdAsync(guidId);

            if (supplier == null)
            {
                _logger.LogWarning("Supplier with ID {Id} not found.", model.Id);
                ModelState.AddModelError("editError", "There isn't such supplier.");
                return View(model);
            }

            supplier.CompanyName = model.CompanyName;
            supplier.ContactName = model.ContactName;
            supplier.ContactTitle = model.ContactTitle;
            supplier.Address = model.Address;
            supplier.City = model.City;
            supplier.Region = model.Region;
            supplier.Country = model.Country;
            supplier.PostalCode = model.PostalCode;
            supplier.Country = model.Country;
            supplier.PhoneNumber = model.PhoneNumber;
            supplier.Fax = model.Fax;
            supplier.Website = model.Website;
            supplier.UpdatedAt = DateTime.UtcNow;

            var isUpdated = await _supplierRepository.UpdateAsync(supplier);
            var affected = await _supplierRepository.SaveChangesAsync();

            if (!isUpdated || affected == 0)
            {
                _logger.LogWarning("Supplier update failed for supplierId: {Id}", model.Id);
                ModelState.AddModelError("updateError", "Supplier not updated.");
                return View(model);
            }

            _logger.LogInformation("Supplier updated successfully for supplierId: {Id}", model.Id);
            return RedirectToAction(nameof(GetAllSuppliers));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string Id)
        {
            _logger.LogInformation("Delete POST action called with supplierId: {Id}", Id);

            if (!Guid.TryParse(Id, out var guidId))
            {
                var errorMessage = $"The id '{Id}' is not a valid GUID.";
                _logger.LogError(errorMessage);
                throw new ArgumentException(errorMessage, nameof(Id));
            }

            var isDeleted = await _supplierRepository.DeleteAsync(guidId);
            await _supplierRepository.SaveChangesAsync();

            if (!isDeleted)
            {
                var errorModel = new ErrorModel { ErrorMessage = "There isn't such supplier." };
                _logger.LogWarning("Supplier with ID {Id} not found for deletion.", Id);
                return View("AdminError", errorModel);
            }

            _logger.LogInformation("Supplier deleted successfully for supplierId: {Id}", Id);
            return RedirectToAction(nameof(GetAllSuppliers));
        }

        [HttpPost]
        public async Task<IActionResult> AssumingDeleted(string Id)
        {
            _logger.LogInformation("AssumingDeleted POST action called with supplierId: {Id}", Id);

            if (!Guid.TryParse(Id, out var guidId))
            {
                var errorMessage = $"The id '{Id}' is not a valid GUID.";
                _logger.LogError(errorMessage);
                throw new ArgumentException(errorMessage, nameof(Id));
            }

            var supplier = await _supplierRepository.GetByIdAsync(guidId);

            if (supplier == null)
            {
                var errorModel = new ErrorModel { ErrorMessage = "There isn't such supplier." };
                _logger.LogWarning("Supplier with ID {Id} not found.", Id);
                return View("AdminError", errorModel);
            }

            supplier.IsDeleted = true;

            var isUpdated = await _supplierRepository.UpdateAsync(supplier);
            await _supplierRepository.SaveChangesAsync();

            if (!isUpdated)
            {
                var errorModel = new ErrorModel { ErrorMessage = "Supplier not updated." };
                _logger.LogWarning("Supplier update to deleted failed for supplierId: {Id}", Id);
                return View("AdminError", errorModel);
            }

            _logger.LogInformation("Supplier marked as deleted successfully for supplierId: {Id}", Id);
            return RedirectToAction(nameof(GetAllSuppliers));
        }
    }
}
