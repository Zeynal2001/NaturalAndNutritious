using Moq;
using NaturalAndNutritious.Business.Abstractions;
using NaturalAndNutritious.Data.Entities;

namespace NaturalAndNutritious.Tests.AdminPanelServices
{
    public class ProductServiceTests
    {
        [Theory]
        [InlineData(100.0, "Percentage", 10, 90.0)] // Yüzde indirim
        [InlineData(100.0, "FixedAmount", 20.0, 80.0)] // Sabit miktar indirim
        public void ApplyDiscount_ReturnsCorrectDiscountedPrice(double originalPrice, string discountType, double discountRate, double expectedDiscountedPrice)
        {
            // Arrange
            var discount = new Discount { DiscountType = discountType, DiscountRate = discountRate };
            var productServiceMock = new Mock<IProductService>(); // Mock ProductService oluştur
            var productService = productServiceMock.Object;

            // Act
            var discountedPrice = productService.ApplyDiscount(originalPrice, discount); // İndirim uygula

            // Assert
            Assert.Equal(expectedDiscountedPrice, discountedPrice); // İndirim uygulanmış fiyatın beklenen fiyata eşit olup olmadığını kontrol et
        }
    }
}

