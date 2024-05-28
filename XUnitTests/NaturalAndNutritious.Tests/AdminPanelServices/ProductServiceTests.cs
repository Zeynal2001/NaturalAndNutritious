using Moq;
using NaturalAndNutritious.Business.Abstractions;
using NaturalAndNutritious.Data.Entities;

namespace NaturalAndNutritious.Tests.AdminPanelServices
{
    public class ProductServiceTests
    {
        [Theory]
        [InlineData(100.0, "Percentage", 10, 90.0)]
        [InlineData(100.0, "FixedAmount", 20.0, 80.0)]
        public void ApplyDiscount_ReturnsCorrectDiscountedPrice(double originalPrice, string discountType, double discountRate, double expectedDiscountedPrice)
        {
            // Arrange
            var discount = new Discount { DiscountType = discountType, DiscountRate = discountRate };
            var productServiceMock = new Mock<IProductService>();
            var productService = productServiceMock.Object;

            // Act
            var discountedPrice = productService.ApplyDiscount(originalPrice, discount); // Endirim tədbiq edilir.

            // Assert
            Assert.Equal(expectedDiscountedPrice, discountedPrice); // Endirimli qiymətin gözlənilən qiymətə bərabər olub-olmadığını yoxlanılır.
        }
    }
}

