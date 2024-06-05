using Microsoft.EntityFrameworkCore;
using NaturalAndNutritious.Business.Abstractions.AdminPanelAbstractions;
using NaturalAndNutritious.Business.Dtos.AdminPanelDtos;
using NaturalAndNutritious.Business.Services.Results;
using NaturalAndNutritious.Data.Abstractions;
using NaturalAndNutritious.Data.Entities;

namespace NaturalAndNutritious.Business.Services.AdminPanelServices
{
    public class SupplierService : ISupplierService
    {
        public SupplierService(ISupplierRepository supplierRepository)
        {
            _supplierRepository = supplierRepository;
        }

        private readonly ISupplierRepository _supplierRepository;

        public async Task<int> TotalSuppliers()
        {
            return await _supplierRepository.Table
                        .OrderByDescending(sc => sc.CreatedAt)
                        .CountAsync();
        }

        public async Task<SupplierServiceResult> CreateSupplier(CreateSupplierDto model)
        {
            var result = 0;
            if (model != null)
            {
                var supplier = new Supplier()
                {
                    Id = Guid.NewGuid(),
                    CompanyName = model.CompanyName,
                    ContactName = model.ContactName,
                    ContactTitle = model.ContactTitle,
                    Address = model.Address,
                    City = model.City,
                    Region = model.Region,
                    PostalCode = model.PostalCode,
                    Country = model.Country,
                    PhoneNumber = model.PhoneNumber,
                    Fax = model.Fax,
                    Website = model.Website,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                };

                await _supplierRepository.CreateAsync(supplier);
                result = await _supplierRepository.SaveChangesAsync();

                if (result == 0)
                {
                    return new SupplierServiceResult
                    {
                        Succeeded = false,
                        IsNull = false,
                        Message = "Supplier is not created, The table is not affected."
                    };
                }

                return new SupplierServiceResult
                {
                    Succeeded = true,
                    IsNull = false,
                    Message = "Supplier created successfully."
                };
            }
            else
            {
                return new SupplierServiceResult { Succeeded = false, IsNull = true, Message = "Model is null." };
            }
        }
    }
}
