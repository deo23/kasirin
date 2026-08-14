namespace KasirIn.UnitTests.Application;

using KasirIn.Application.Reports.Queries.ExportProfitReportToExcel;
using KasirIn.Domain.Entities;
using KasirIn.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class ExportProfitReportToExcelQueryHandlerTests
{
    private readonly KasirInDbContext _context;

    public ExportProfitReportToExcelQueryHandlerTests()
    {
        var options = new DbContextOptionsBuilder<KasirInDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new KasirInDbContext(options);
    }

    [Fact]
    public async Task Handle_ShouldReturnValidExcelByteArray()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var category = new Category { Id = Guid.NewGuid(), TenantId = tenantId, Name = "Snacks" };
        var product = new Product { Id = Guid.NewGuid(), TenantId = tenantId, CategoryId = category.Id, Name = "Keripik Kentang", SKU = "SNK-001", CostPrice = 5000, SellingPrice = 8000, StockQuantity = 20 };
        _context.Categories.Add(category);
        _context.Products.Add(product);

        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            InvoiceNumber = "TRX-TEST-001",
            TransactionDate = DateTime.Now,
            TotalAmount = 8000,
            PaidAmount = 10000,
            ChangeAmount = 2000,
            PaymentMethod = "CASH",
            TransactionItems = new List<TransactionItem>
            {
                new TransactionItem { Id = Guid.NewGuid(), ProductId = product.Id, Quantity = 1, UnitPrice = 8000, CostPrice = 5000, SubTotal = 8000, Profit = 3000 }
            }
        };
        _context.Transactions.Add(transaction);

        var debt = new DebtRecord
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            CustomerName = "Pak Budi",
            CustomerPhone = "081234567890",
            TotalDebt = 50000,
            PaidDebt = 10000
        };
        _context.DebtRecords.Add(debt);

        await _context.SaveChangesAsync();

        var handler = new ExportProfitReportToExcelQueryHandler(_context);
        var query = new ExportProfitReportToExcelQuery(tenantId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Length > 0, "File Excel byte array tidak boleh kosong.");
    }
}
