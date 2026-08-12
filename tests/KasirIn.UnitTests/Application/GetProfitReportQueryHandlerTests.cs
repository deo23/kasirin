namespace KasirIn.UnitTests.Application;

using KasirIn.Application.Reports.Queries.GetProfitReport;
using KasirIn.Domain.Entities;
using KasirIn.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class GetProfitReportQueryHandlerTests
{
    private static KasirInDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<KasirInDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new KasirInDbContext(options);
    }

    [Fact]
    public async Task Handle_ValidDateRange_ShouldCalculateRevenueCogsAndProfitCorrectly()
    {
        // Arrange
        using var context = CreateDbContext();
        var tenantId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var baseDate = new DateTime(2026, 8, 1, 10, 0, 0, DateTimeKind.Utc);

        var product = new Product
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Name = "Roti Bakar",
            SKU = "RTB-001",
            CostPrice = 6000m,
            SellingPrice = 10000m,
            StockQuantity = 100
        };
        context.Products.Add(product);

        // Transaction 1: 2 items = Revenue 20k, COGS 12k
        var t1 = new Transaction
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            UserId = userId,
            InvoiceNumber = "INV-001",
            TransactionDate = baseDate.AddDays(1),
            TotalAmount = 20000m,
            PaidAmount = 20000m,
            ChangeAmount = 0m,
            PaymentMethod = "CASH",
            TransactionItems = new List<TransactionItem>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    ProductId = product.Id,
                    Quantity = 2,
                    UnitPrice = 10000m,
                    CostPrice = 6000m,
                    SubTotal = 20000m,
                    Profit = 8000m
                }
            }
        };

        // Transaction 2: 3 items = Revenue 30k, COGS 18k
        var t2 = new Transaction
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            UserId = userId,
            InvoiceNumber = "INV-002",
            TransactionDate = baseDate.AddDays(2),
            TotalAmount = 30000m,
            PaidAmount = 30000m,
            ChangeAmount = 0m,
            PaymentMethod = "QRIS",
            TransactionItems = new List<TransactionItem>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    ProductId = product.Id,
                    Quantity = 3,
                    UnitPrice = 10000m,
                    CostPrice = 6000m,
                    SubTotal = 30000m,
                    Profit = 12000m
                }
            }
        };

        // Transaction 3: Outside date range (AddDays(10))
        var t3 = new Transaction
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            UserId = userId,
            InvoiceNumber = "INV-003",
            TransactionDate = baseDate.AddDays(10),
            TotalAmount = 50000m,
            PaidAmount = 50000m,
            TransactionItems = new List<TransactionItem>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    ProductId = product.Id,
                    Quantity = 5,
                    UnitPrice = 10000m,
                    CostPrice = 6000m,
                    SubTotal = 50000m,
                    Profit = 20000m
                }
            }
        };

        context.Transactions.AddRange(t1, t2, t3);
        await context.SaveChangesAsync();

        var handler = new GetProfitReportQueryHandler(context);
        var query = new GetProfitReportQuery(tenantId, baseDate, baseDate.AddDays(5));

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(tenantId, result.TenantId);
        Assert.Equal(2, result.TotalTransactions);
        Assert.Equal(50000m, result.TotalRevenue); // 20k + 30k
        Assert.Equal(30000m, result.TotalCogs);    // 12k + 18k
        Assert.Equal(20000m, result.NetProfit);    // 50k - 30k
    }

    [Fact]
    public async Task Handle_NoTransactions_ShouldReturnZeroReport()
    {
        // Arrange
        using var context = CreateDbContext();
        var tenantId = Guid.NewGuid();
        var handler = new GetProfitReportQueryHandler(context);
        var query = new GetProfitReportQuery(tenantId, DateTime.UtcNow.AddDays(-7), DateTime.UtcNow);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(tenantId, result.TenantId);
        Assert.Equal(0, result.TotalTransactions);
        Assert.Equal(0m, result.TotalRevenue);
        Assert.Equal(0m, result.TotalCogs);
        Assert.Equal(0m, result.NetProfit);
    }
}
