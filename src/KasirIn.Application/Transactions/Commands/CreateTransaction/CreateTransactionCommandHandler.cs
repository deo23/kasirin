namespace KasirIn.Application.Transactions.Commands.CreateTransaction;

using KasirIn.Application.Common.Interfaces;
using KasirIn.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class CreateTransactionCommandHandler : IRequestHandler<CreateTransactionCommand, Guid>
{
    private readonly IKasirInDbContext _context;

    public CreateTransactionCommandHandler(IKasirInDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
    {
        if (request.Items == null || !request.Items.Any())
        {
            throw new InvalidOperationException("Transaksi harus memiliki minimal 1 item.");
        }

        var productIds = request.Items.Select(i => i.ProductId).Distinct().ToList();

        var products = await _context.Products
            .Where(p => p.TenantId == request.TenantId && productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, cancellationToken);

        var transactionItems = new List<TransactionItem>();

        foreach (var itemDto in request.Items)
        {
            if (!products.TryGetValue(itemDto.ProductId, out var product))
            {
                throw new KeyNotFoundException($"Produk dengan ID {itemDto.ProductId} tidak ditemukan.");
            }

            if (product.StockQuantity < itemDto.Quantity)
            {
                throw new InvalidOperationException($"Stok produk '{product.Name}' tidak mencukupi (Tersedia: {product.StockQuantity}, Diminta: {itemDto.Quantity}).");
            }

            // Potong stok produk
            product.StockQuantity -= itemDto.Quantity;

            var transactionItem = new TransactionItem
            {
                Id = Guid.NewGuid(),
                ProductId = product.Id,
                Quantity = itemDto.Quantity,
                UnitPrice = product.SellingPrice,
                CostPrice = product.CostPrice,
                SubTotal = itemDto.Quantity * product.SellingPrice,
                Profit = (product.SellingPrice - product.CostPrice) * itemDto.Quantity
            };

            transactionItems.Add(transactionItem);
        }

        var totalAmount = transactionItems.Sum(i => i.SubTotal);

        if (request.PaidAmount < totalAmount)
        {
            throw new InvalidOperationException($"Jumlah pembayaran ({request.PaidAmount:N0}) kurang dari total transaksi ({totalAmount:N0}).");
        }

        var invoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString()[..4].ToUpper()}";

        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            TenantId = request.TenantId,
            UserId = request.UserId,
            InvoiceNumber = invoiceNumber,
            TransactionDate = DateTime.UtcNow,
            TotalAmount = totalAmount,
            PaidAmount = request.PaidAmount,
            ChangeAmount = request.PaidAmount - totalAmount,
            PaymentMethod = request.PaymentMethod,
            TransactionItems = transactionItems
        };

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync(cancellationToken);

        return transaction.Id;
    }
}
