using KasirIn.Application.Common.Interfaces;
using KasirIn.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace KasirIn.Infrastructure.Persistence;

public class KasirInDbContext : DbContext, IKasirInDbContext
{
    public KasirInDbContext(DbContextOptions<KasirInDbContext> options) : base(options)
    {
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<TransactionItem> TransactionItems => Set<TransactionItem>();
    public DbSet<DebtRecord> DebtRecords => Set<DebtRecord>();

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);
        
        // Configure decimal(18,2) precision for all decimal properties (prices, amounts, profit, debt)
        configurationBuilder.Properties<decimal>().HavePrecision(18, 2);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Tenant>(builder =>
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Name).IsRequired().HasMaxLength(150);
        });

        modelBuilder.Entity<User>(builder =>
        {
            builder.HasKey(u => u.Id);
            builder.Property(u => u.FullName).IsRequired().HasMaxLength(150);
            builder.Property(u => u.Email).IsRequired().HasMaxLength(150);
            builder.HasOne(u => u.Tenant)
                   .WithMany(t => t.Users)
                   .HasForeignKey(u => u.TenantId)
                   .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Category>(builder =>
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
            builder.HasOne(c => c.Tenant)
                   .WithMany(t => t.Categories)
                   .HasForeignKey(c => c.TenantId)
                   .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Product>(builder =>
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Name).IsRequired().HasMaxLength(150);
            builder.Property(p => p.SKU).IsRequired().HasMaxLength(50);
            builder.Ignore(p => p.IsLowStock);
            builder.HasOne(p => p.Tenant)
                   .WithMany(t => t.Products)
                   .HasForeignKey(p => p.TenantId)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(p => p.Category)
                   .WithMany(c => c.Products)
                   .HasForeignKey(p => p.CategoryId)
                   .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Transaction>(builder =>
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.InvoiceNumber).IsRequired().HasMaxLength(50);
            builder.Ignore(t => t.TotalProfit);
            builder.HasOne(t => t.Tenant)
                   .WithMany(ten => ten.Transactions)
                   .HasForeignKey(t => t.TenantId)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(t => t.User)
                   .WithMany()
                   .HasForeignKey(t => t.UserId)
                   .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<TransactionItem>(builder =>
        {
            builder.HasKey(ti => ti.Id);
            builder.HasOne(ti => ti.Transaction)
                   .WithMany(t => t.TransactionItems)
                   .HasForeignKey(ti => ti.TransactionId)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(ti => ti.Product)
                   .WithMany()
                   .HasForeignKey(ti => ti.ProductId)
                   .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<DebtRecord>(builder =>
        {
            builder.HasKey(d => d.Id);
            builder.Property(d => d.CustomerName).IsRequired().HasMaxLength(150);
            builder.HasOne(d => d.Tenant)
                   .WithMany(t => t.DebtRecords)
                   .HasForeignKey(d => d.TenantId)
                   .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
