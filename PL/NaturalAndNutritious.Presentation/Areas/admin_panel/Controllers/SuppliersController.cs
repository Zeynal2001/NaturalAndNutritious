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
        public SuppliersController(ISupplierRepository supplierRepository, ISupplierService supplierService)
        {
            _supplierRepository = supplierRepository;
            _supplierService = supplierService;
        }

        private readonly ISupplierRepository _supplierRepository;
        private readonly ISupplierService _supplierService;

        [Area("admin_panel")]
        [Authorize(Roles = nameof(RoleTypes.Admin), AuthenticationSchemes = "AdminAuth")]
        public async Task<IActionResult> GetAllSuppliers(int page = 1, int pageSize = 5)
        {
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

            return View(vm);
        }


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateSupplierDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _supplierService.CreateSupplier(model);

            if (!result.Succeeded)
            {
                var error = new ErrorModel { ErrorMessage = result.Message };
                return View("AdminError", error);
            }

            return RedirectToAction(nameof(GetAllSuppliers));
        }

        public async Task<IActionResult> Update(string Id)
        {
            if (!Guid.TryParse(Id, out var guidId))
            {
                throw new ArgumentException($"The id '{Id}' is not a valid GUID.", nameof(Id));
            }

            var supplier = await _supplierRepository.GetByIdAsync(guidId);

            if (supplier == null)
            {
                var errorModel = new ErrorModel()
                {
                    ErrorMessage = "There isn't such supplier"
                };

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

            ViewData["hasError"] = false;

            return View(supplierDetails);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UpdateSupplierDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            int affected = 0;

            if (!Guid.TryParse(model.Id, out var guidId))
            {
                throw new ArgumentException($"The id '{model.Id}' is not a valid GUID.", nameof(model.Id));
            }

            var supplier = await _supplierRepository.GetByIdAsync(guidId);

            if (supplier == null)
            {
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
            affected = await _supplierRepository.SaveChangesAsync();

            if (isUpdated == false && affected == 0)
            {
                ModelState.AddModelError("updateError", "Supplier not updated.");

                return View(model);
            }

            return RedirectToAction(nameof(GetAllSuppliers));
        }


        [HttpPost]
        public async Task<IActionResult> Delete(string Id)
        {
            if (!Guid.TryParse(Id, out var guidId))
            {
                throw new ArgumentException($"The id '{Id}' is not a valid GUID.", nameof(Id));
            }

            var isDeleted = await _supplierRepository.DeleteAsync(guidId);
            await _supplierRepository.SaveChangesAsync();

            if (isDeleted == false)
            {
                var errorModel = new ErrorModel();
                errorModel.ErrorMessage = "There isn't such supplier.";

                return View("AdminError", errorModel);
            }

            return RedirectToAction(nameof(GetAllSuppliers));
        }

        [HttpPost]
        public async Task<IActionResult> AssumingDeleted(string Id)
        {
            if (!Guid.TryParse(Id, out var guidId))
            {
                throw new ArgumentException($"The id '{Id}' is not a valid GUID.", nameof(Id));
            }
            var supplier = await _supplierRepository.GetByIdAsync(guidId);

            if (supplier == null)
            {
                var errorModel = new ErrorModel();
                errorModel.ErrorMessage = "There isn't such supplier.";

                return View("AdminError", errorModel);
            }

            supplier.IsDeleted = true;

            var isUpdated = await _supplierRepository.UpdateAsync(supplier);
            await _supplierRepository.SaveChangesAsync();

            if (isUpdated == false)
            {
                var errorModel = new ErrorModel();
                errorModel.ErrorMessage = "Supplier not updated.";

                return View("AdminError", errorModel);
            }

            return RedirectToAction(nameof(GetAllSuppliers));
        }
    }
}
