using NaturalAndNutritious.Business.Abstractions.AdminPanelAbstractions;
using NaturalAndNutritious.Business.Dtos.AdminPanelDtos;
using NaturalAndNutritious.Business.Services.Results;
using NaturalAndNutritious.Data.Abstractions;
using NaturalAndNutritious.Data.Entities;

namespace NaturalAndNutritious.Business.Services.AdminPanelServices
{
    public class DiscountService : IDiscountService
    {
        public DiscountService(IDiscountRepository discountRepository, IProductRepository productRepository)
        {
            _discountRepository = discountRepository;
            _productRepository = productRepository;
        }

        private readonly IDiscountRepository _discountRepository;
        private readonly IProductRepository _productRepository;

        public async Task<DiscountServiceResult> CreateDiscount(CreateDiscountDto model)
        {
            var affected = 0;
            if (model != null)
            {
                if (!Guid.TryParse(model.ProductId, out var productId))
                {
                    throw new ArgumentException($"The id '{model.ProductId}' is not a valid GUID.", nameof(model.ProductId));
                }

                var givenProduct = await _productRepository.GetByIdAsync(productId);
                if (givenProduct == null)
                {
                    //throw new Exception("Product not found");
                    return new DiscountServiceResult { Succeeded = false, IsNull = true, Message = "Product not found!" };
                }

                var discount = new Discount
                {
                    Id = Guid.NewGuid(),
                    ProductId = givenProduct.Id,
                    DiscountRate = model.DiscountRate, //Amount
                    DiscountType = model.DiscountType,
                    CreatedAt = DateTime.UtcNow,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    IsDeleted = false,
                    //IsActive = true,
                };


                await _discountRepository.CreateAsync(discount);
                affected = await _discountRepository.SaveChangesAsync();

                if (affected == 0)
                {
                    return new DiscountServiceResult
                    {
                        Succeeded = false,
                        IsNull = false,
                        Message = "Discount is not created, The table is not affected."
                    };
                }

                return new DiscountServiceResult
                {
                    Succeeded = true,
                    IsNull = false,
                    Message = "Discount created successfully."
                };
            }
            else
            {
                return new DiscountServiceResult { Succeeded = false, IsNull = true, Message = "Model is null." };
            }
        }
    }
}
