namespace KasirIn.UnitTests.Application;

using KasirIn.Application.Transactions.Commands.CreateTransaction;
using KasirIn.Domain.Entities;
using KasirIn.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class CreateTransactionCommandHandlerTests
{
    private static KasirInDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<KasirInDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new KasirInDbContext(options);
    }

    [Fact]
    public async Task Handle_ValidTransaction_ShouldDeductStockAndSaveTransaction()
    {
        // Arrange
        using var context = CreateDbContext();
        var tenantId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();

        var product = new Product
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            CategoryId = categoryId,
            Name = "Kopi Susu",
            SKU = "KPS-001",
            CostPrice = 10000m,
            SellingPrice = 15000m,
            StockQuantity = 10,
            MinStockThreshold = 2
        };

        context.Products.Add(product);
        await context.SaveChangesAsync();

        var handler = new CreateTransactionCommandHandler(context);

        var command = new CreateTransactionCommand
        {
            TenantId = tenantId,
            UserId = userId,
            PaymentMethod = "CASH",
            PaidAmount = 50000m,
            Items = new List<CreateTransactionItemCommandDto>
            {
                new(product.Id, 2)
            }
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, result);

        var updatedProduct = await context.Products.FindAsync(product.Id);
        Assert.NotNull(updatedProduct);
        Assert.Equal(8, updatedProduct.StockQuantity);

        var transaction = await context.Transactions
            .Include(t => t.TransactionItems)
            .FirstOrDefaultAsync(t => t.Id == result);

        Assert.NotNull(transaction);
        Assert.Equal(30000m, transaction.TotalAmount);
        Assert.Equal(50000m, transaction.PaidAmount);
        Assert.Equal(20000m, transaction.ChangeAmount);
        Assert.Single(transaction.TransactionItems);

        var item = transaction.TransactionItems.First();
        Assert.Equal(product.Id, item.ProductId);
        Assert.Equal(2, item.Quantity);
        Assert.Equal(15000m, item.UnitPrice);
        Assert.Equal(10000m, item.CostPrice);
        Assert.Equal(30000m, item.SubTotal);
        Assert.Equal(10000m, item.Profit);
    }

    [Fact]
    public async Task Handle_InsufficientStock_ShouldThrowInvalidOperationException()
    {
        // Arrange
        using var context = CreateDbContext();
        var tenantId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var product = new Product
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Name = "Teh Manis",
            SKU = "THM-001",
            CostPrice = 2000m,
            SellingPrice = 5000m,
            StockQuantity = 1
        };

        context.Products.Add(product);
        await context.SaveChangesAsync();

        var handler = new CreateTransactionCommandHandler(context);

        var command = new CreateTransactionCommand
        {
            TenantId = tenantId,
            UserId = userId,
            PaidAmount = 20000m,
            Items = new List<CreateTransactionItemCommandDto>
            {
                new(product.Id, 5)
            }
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ProductNotFound_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        using var context = CreateDbContext();
        var handler = new CreateTransactionCommandHandler(context);

        var command = new CreateTransactionCommand
        {
            TenantId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            PaidAmount = 20000m,
            Items = new List<CreateTransactionItemCommandDto>
            {
                new(Guid.NewGuid(), 1)
            }
        };

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_PaidAmountLessThanTotal_ShouldThrowInvalidOperationException()
    {
        // Arrange
        using var context = CreateDbContext();
        var tenantId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var product = new Product
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Name = "Nasi Goreng",
            SKU = "NSG-001",
            CostPrice = 12000m,
            SellingPrice = 20000m,
            StockQuantity = 10
        };

        context.Products.Add(product);
        await context.SaveChangesAsync();

        var handler = new CreateTransactionCommandHandler(context);

        var command = new CreateTransactionCommand
        {
            TenantId = tenantId,
            UserId = userId,
            PaidAmount = 15000m, // Less than TotalAmount (20,000)
            Items = new List<CreateTransactionItemCommandDto>
            {
                new(product.Id, 1)
            }
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));
    }
}
