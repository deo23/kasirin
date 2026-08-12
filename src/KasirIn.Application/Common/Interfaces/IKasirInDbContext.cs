namespace KasirIn.Application.Common.Interfaces;

using KasirIn.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public interface IKasirInDbContext
{
    DbSet<Tenant> Tenants { get; }
    DbSet<User> Users { get; }
    DbSet<Category> Categories { get; }
    DbSet<Product> Products { get; }
    DbSet<Transaction> Transactions { get; }
    DbSet<TransactionItem> TransactionItems { get; }
    DbSet<DebtRecord> DebtRecords { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
