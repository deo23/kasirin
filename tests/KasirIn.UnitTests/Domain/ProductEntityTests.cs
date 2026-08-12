using KasirIn.Domain.Entities;
using Xunit;

namespace KasirIn.UnitTests.Domain;

public class ProductEntityTests
{
    [Theory]
    [InlineData(2, 5, true)]   // Stock < Threshold => Low stock
    [InlineData(5, 5, true)]   // Stock == Threshold => Low stock
    [InlineData(0, 5, true)]   // Stock 0 <= Threshold => Low stock
    [InlineData(10, 5, false)] // Stock > Threshold => Normal stock
    [InlineData(6, 5, false)]  // Stock > Threshold => Normal stock
    public void IsLowStock_ShouldReturnExpectedResult_BasedOnStockAndThreshold(int stockQuantity, int minStockThreshold, bool expectedIsLowStock)
    {
        // Arrange
        var product = new Product
        {
            Name = "Kopi Susu",
            SKU = "KPS-001",
            CostPrice = 10000,
            SellingPrice = 15000,
            StockQuantity = stockQuantity,
            MinStockThreshold = minStockThreshold
        };

        // Act
        var result = product.IsLowStock;

        // Assert
        Assert.Equal(expectedIsLowStock, result);
    }
}
