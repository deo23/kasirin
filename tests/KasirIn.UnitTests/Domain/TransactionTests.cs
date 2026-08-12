using KasirIn.Domain.Entities;
using Xunit;

namespace KasirIn.UnitTests.Domain;

public class TransactionTests
{
    [Fact]
    public void TransactionItem_SubTotalAndProfit_ShouldCalculateCorrectly()
    {
        // Arrange
        var item = new TransactionItem
        {
            Quantity = 3,
            UnitPrice = 15000m,
            CostPrice = 10000m
        };

        // Act
        var subTotal = item.SubTotal;
        var profit = item.Profit;

        // Assert
        Assert.Equal(45000m, subTotal); // 3 * 15,000 = 45,000
        Assert.Equal(15000m, profit);   // (15,000 - 10,000) * 3 = 15,000
    }

    [Fact]
    public void Transaction_TotalAmountAndProfit_ShouldCalculateFromItems()
    {
        // Arrange
        var item1 = new TransactionItem
        {
            Quantity = 2,
            UnitPrice = 10000m,
            CostPrice = 7000m
        }; // SubTotal = 20,000 | Profit = 6,000

        var item2 = new TransactionItem
        {
            Quantity = 1,
            UnitPrice = 50000m,
            CostPrice = 35000m
        }; // SubTotal = 50,000 | Profit = 15,000

        var transaction = new Transaction
        {
            InvoiceNumber = "INV-20260812-001",
            PaidAmount = 100000m,
            TransactionItems = new List<TransactionItem> { item1, item2 }
        };

        // Act
        var totalAmount = transaction.TotalAmount;
        var totalProfit = transaction.TotalProfit;
        var changeAmount = transaction.ChangeAmount;

        // Assert
        Assert.Equal(70000m, totalAmount);   // 20,000 + 50,000 = 70,000
        Assert.Equal(21000m, totalProfit);    // 6,000 + 15,000 = 21,000
        Assert.Equal(30000m, changeAmount);   // 100,000 - 70,000 = 30,000
    }

    [Fact]
    public void Transaction_RecalculateTotals_ShouldUpdateTotalAndChangeAmount()
    {
        // Arrange
        var item = new TransactionItem
        {
            Quantity = 4,
            UnitPrice = 25000m,
            CostPrice = 18000m
        }; // SubTotal = 100,000

        var transaction = new Transaction
        {
            InvoiceNumber = "INV-20260812-002",
            PaidAmount = 120000m,
            TransactionItems = new List<TransactionItem> { item }
        };

        // Act
        transaction.RecalculateTotals();

        // Assert
        Assert.Equal(100000m, transaction.TotalAmount);
        Assert.Equal(20000m, transaction.ChangeAmount);
    }
}
