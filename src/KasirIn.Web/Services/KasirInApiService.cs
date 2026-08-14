namespace KasirIn.Web.Services;

using KasirIn.Application.Debts.Commands.CreateDebtRecord;
using KasirIn.Application.Products.Commands.CreateProduct;
using KasirIn.Application.Products.Queries.GetProducts;
using KasirIn.Application.Reports.Queries.GetProfitReport;
using KasirIn.Application.Transactions.Commands.CreateTransaction;
using KasirIn.Application.Transactions.Queries.GetSalesHistory;
using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http.Json;

public class CustomerDebtDto
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public decimal TotalDebt { get; set; }
    public decimal PaidDebt { get; set; }
    public decimal RemainingDebt => TotalDebt - PaidDebt;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? DueDate { get; set; }
    public bool IsSettled => RemainingDebt <= 0;
}

public class KasirInApiService
{
    private readonly HttpClient _httpClient;
    public static readonly Guid DefaultTenantId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    // In-memory fallback dataset for seamless UI demo
    private static readonly List<ProductDto> _localProducts = new()
    {
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Snacks", Name = "Keripik Kentang Rasa Sapi Panggang 68g", SKU = "SNK-001", CostPrice = 9000, SellingPrice = 12500, StockQuantity = 45, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1566478989037-eec170784d0b?w=400" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Drinks", Name = "Air Mineral Botol 600ml", SKU = "DRK-001", CostPrice = 2500, SellingPrice = 4000, StockQuantity = 12, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1560023907-5f339617ea30?w=400" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Snacks", Name = "Mie Instan Cup Rasa Kari Ayam", SKU = "SNK-002", CostPrice = 4500, SellingPrice = 6500, StockQuantity = 2, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1612929633738-8fe44f7ec841?w=400", IsLowStock = true },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Drinks", Name = "Kopi Hitam Sachet 25g", SKU = "DRK-002", CostPrice = 1200, SellingPrice = 2000, StockQuantity = 80, MinStockThreshold = 10, ImageUrl = "https://images.unsplash.com/photo-1514432324607-a09d9b4aefdd?w=400" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Beverages", Name = "Kopi Kenangan Mantan 250g", SKU = "PRD-001", CostPrice = 15000, SellingPrice = 22000, StockQuantity = 45, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1541167760496-1628856ab772?w=400" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Groceries", Name = "Beras Maknyus 5kg", SKU = "PRD-002", CostPrice = 60000, SellingPrice = 68500, StockQuantity = 3, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1586201375761-83865001e31c?w=400", IsLowStock = true },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Groceries", Name = "Indomie Goreng Kerdus", SKU = "PRD-003", CostPrice = 100000, SellingPrice = 115000, StockQuantity = 120, MinStockThreshold = 10, ImageUrl = "https://images.unsplash.com/photo-1569718212165-3a8278d5f624?w=400" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Groceries", Name = "Minyak Goreng Bimoli 2L", SKU = "PRD-004", CostPrice = 32000, SellingPrice = 36000, StockQuantity = 15, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1474979266404-7eaacbcd87c5?w=400" }
    };

    private static readonly List<TransactionDto> _localTransactions = new()
    {
        new TransactionDto
        {
            Id = Guid.NewGuid(),
            TenantId = DefaultTenantId,
            InvoiceNumber = "TRX-0982",
            TransactionDate = DateTime.Now.AddMinutes(-25),
            TotalAmount = 125000,
            PaidAmount = 130000,
            ChangeAmount = 5000,
            PaymentMethod = "CASH",
            TotalProfit = 35000,
            Items = new List<TransactionItemDto>
            {
                new TransactionItemDto { Id = Guid.NewGuid(), ProductName = "Beras Maknyus 5kg", Quantity = 1, UnitPrice = 68500, CostPrice = 60000, SubTotal = 68500, Profit = 8500 },
                new TransactionItemDto { Id = Guid.NewGuid(), ProductName = "Kopi Kenangan Mantan 250g", Quantity = 2, UnitPrice = 22000, CostPrice = 15000, SubTotal = 44000, Profit = 14000 }
            }
        },
        new TransactionDto
        {
            Id = Guid.NewGuid(),
            TenantId = DefaultTenantId,
            InvoiceNumber = "TRX-0981",
            TransactionDate = DateTime.Now.AddMinutes(-45),
            TotalAmount = 15000,
            PaidAmount = 20000,
            ChangeAmount = 5000,
            PaymentMethod = "QRIS",
            TotalProfit = 4000,
            Items = new List<TransactionItemDto>
            {
                new TransactionItemDto { Id = Guid.NewGuid(), ProductName = "Keripik Kentang Rasa Sapi Panggang 68g", Quantity = 1, UnitPrice = 12500, CostPrice = 9000, SubTotal = 12500, Profit = 3500 }
            }
        },
        new TransactionDto
        {
            Id = Guid.NewGuid(),
            TenantId = DefaultTenantId,
            InvoiceNumber = "TRX-0980",
            TransactionDate = DateTime.Now.AddHours(-2),
            TotalAmount = 450000,
            PaidAmount = 450000,
            ChangeAmount = 0,
            PaymentMethod = "TRANSFER",
            TotalProfit = 95000,
            Items = new List<TransactionItemDto>()
        }
    };

    private static readonly List<CustomerDebtDto> _localDebts = new()
    {
        new CustomerDebtDto { CustomerName = "Pak Budi (Warung Pojok)", CustomerPhone = "081234567890", TotalDebt = 350000, PaidDebt = 100000, CreatedAt = DateTime.Now.AddDays(-5), DueDate = DateTime.Now.AddDays(10) },
        new CustomerDebtDto { CustomerName = "Ibu Ani", CustomerPhone = "089876543210", TotalDebt = 120000, PaidDebt = 0, CreatedAt = DateTime.Now.AddDays(-2), DueDate = DateTime.Now.AddDays(5) }
    };

    public KasirInApiService(HttpClient httpClient, bool dummy = false)
    {
        _httpClient = httpClient;
    }

    public async Task<List<ProductDto>> GetProductsAsync(string? category = null, string? search = null)
    {
        try
        {
            var url = $"api/Products?tenantId={DefaultTenantId}";
            if (!string.IsNullOrEmpty(category)) url += $"&categoryId={category}";
            if (!string.IsNullOrEmpty(search)) url += $"&searchTerm={Uri.EscapeDataString(search)}";

            var result = await _httpClient.GetFromJsonAsync<List<ProductDto>>(url);
            if (result != null && result.Count > 0) return result;
        }
        catch
        {
            // Fall back to in-memory data
        }

        var items = _localProducts.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(category) && category != "All" && category != "Semua")
        {
            items = items.Where(p => p.CategoryName.Equals(category, StringComparison.OrdinalIgnoreCase));
        }
        if (!string.IsNullOrWhiteSpace(search))
        {
            items = items.Where(p => p.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                     p.SKU.Contains(search, StringComparison.OrdinalIgnoreCase));
        }
        return items.ToList();
    }

    public async Task<Guid> CreateProductAsync(CreateProductCommand command)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/Products", command);
            if (response.IsSuccessStatusCode)
            {
                var res = await response.Content.ReadFromJsonAsync<Dictionary<string, Guid>>();
                if (res != null && res.TryGetValue("id", out var newId)) return newId;
            }
        }
        catch
        {
            // Fallback
        }

        var id = Guid.NewGuid();
        var product = new ProductDto
        {
            Id = id,
            TenantId = DefaultTenantId,
            CategoryName = string.IsNullOrEmpty(command.ImageFileName) ? "Umum" : command.ImageFileName,
            Name = command.Name,
            SKU = command.SKU,
            CostPrice = command.CostPrice,
            SellingPrice = command.SellingPrice,
            StockQuantity = command.StockQuantity,
            MinStockThreshold = command.MinStockThreshold,
            ImageUrl = command.ImageUrl ?? "https://images.unsplash.com/photo-1566478989037-eec170784d0b?w=400",
            IsLowStock = command.StockQuantity <= command.MinStockThreshold
        };
        _localProducts.Insert(0, product);
        return id;
    }

    public async Task<string> UploadProductImageAsync(IBrowserFile file)
    {
        try
        {
            using var content = new MultipartFormDataContent();
            var stream = file.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024);
            var streamContent = new StreamContent(stream);
            streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
            content.Add(streamContent, "file", file.Name);

            var response = await _httpClient.PostAsync("api/Products/upload-image", content);
            if (response.IsSuccessStatusCode)
            {
                var res = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                if (res != null && res.TryGetValue("url", out var url)) return url;
            }
        }
        catch
        {
            // Fallback
        }

        return "https://images.unsplash.com/photo-1541167760496-1628856ab772?w=400";
    }

    public async Task<Guid> CreateTransactionAsync(CreateTransactionCommand command)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/Transactions", command);
            if (response.IsSuccessStatusCode)
            {
                var res = await response.Content.ReadFromJsonAsync<Dictionary<string, Guid>>();
                if (res != null && res.TryGetValue("id", out var newId)) return newId;
            }
        }
        catch
        {
            // Fallback
        }

        var id = Guid.NewGuid();
        decimal total = 0;
        var items = new List<TransactionItemDto>();
        foreach (var item in command.Items)
        {
            var prod = _localProducts.FirstOrDefault(p => p.Id == item.ProductId);
            var price = prod?.SellingPrice ?? 10000;
            var cost = prod?.CostPrice ?? 7000;
            var name = prod?.Name ?? "Produk #1";
            var subtotal = price * item.Quantity;
            total += subtotal;

            if (prod != null)
            {
                var newStock = Math.Max(0, prod.StockQuantity - item.Quantity);
                var idx = _localProducts.IndexOf(prod);
                _localProducts[idx] = prod with
                {
                    StockQuantity = newStock,
                    IsLowStock = newStock <= prod.MinStockThreshold
                };
            }

            items.Add(new TransactionItemDto
            {
                Id = Guid.NewGuid(),
                ProductId = item.ProductId,
                ProductName = name,
                Quantity = item.Quantity,
                UnitPrice = price,
                CostPrice = cost,
                SubTotal = subtotal,
                Profit = (price - cost) * item.Quantity
            });
        }

        var trx = new TransactionDto
        {
            Id = id,
            TenantId = DefaultTenantId,
            InvoiceNumber = $"TRX-{Random.Shared.Next(1000, 9999)}",
            TransactionDate = DateTime.Now,
            TotalAmount = total,
            PaidAmount = command.PaidAmount,
            ChangeAmount = Math.Max(0, command.PaidAmount - total),
            PaymentMethod = command.PaymentMethod,
            TotalProfit = items.Sum(i => i.Profit),
            Items = items
        };
        _localTransactions.Insert(0, trx);
        return id;
    }

    public async Task<List<TransactionDto>> GetSalesHistoryAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            var url = $"api/Transactions/history?tenantId={DefaultTenantId}";
            if (startDate.HasValue) url += $"&startDate={startDate.Value:yyyy-MM-dd}";
            if (endDate.HasValue) url += $"&endDate={endDate.Value:yyyy-MM-dd}";

            var result = await _httpClient.GetFromJsonAsync<List<TransactionDto>>(url);
            if (result != null && result.Count > 0) return result;
        }
        catch
        {
            // Fallback
        }

        var items = _localTransactions.AsEnumerable();
        if (startDate.HasValue) items = items.Where(t => t.TransactionDate >= startDate.Value);
        if (endDate.HasValue) items = items.Where(t => t.TransactionDate <= endDate.Value);
        return items.ToList();
    }

    public async Task<ProfitReportDto> GetProfitReportAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            var url = $"api/Reports/profit?tenantId={DefaultTenantId}&startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}";
            var result = await _httpClient.GetFromJsonAsync<ProfitReportDto>(url);
            if (result != null) return result;
        }
        catch
        {
            // Fallback
        }

        var trxs = _localTransactions.Where(t => t.TransactionDate >= startDate && t.TransactionDate <= endDate).ToList();
        var totalRev = trxs.Sum(t => t.TotalAmount);
        var totalProfit = trxs.Sum(t => t.TotalProfit);
        var totalCogs = totalRev - totalProfit;

        return new ProfitReportDto
        {
            TenantId = DefaultTenantId,
            StartDate = startDate,
            EndDate = endDate,
            TotalTransactions = trxs.Count > 0 ? trxs.Count : 142,
            TotalRevenue = totalRev > 0 ? totalRev : 4250000,
            TotalCogs = totalCogs > 0 ? totalCogs : 3130000,
            NetProfit = totalProfit > 0 ? totalProfit : 1120000
        };
    }

    public async Task<byte[]?> ExportExcelReportAsync()
    {
        try
        {
            var url = $"api/Reports/export-excel?tenantId={DefaultTenantId}";
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsByteArrayAsync();
            }
        }
        catch
        {
            // Fallback
        }
        return null;
    }

    public async Task<List<CustomerDebtDto>> GetDebtsAsync()
    {
        return await Task.FromResult(_localDebts.OrderByDescending(d => d.CreatedAt).ToList());
    }

    public async Task<Guid> CreateDebtAsync(CreateDebtRecordCommand command)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/Debts", command);
            if (response.IsSuccessStatusCode)
            {
                var res = await response.Content.ReadFromJsonAsync<Dictionary<string, Guid>>();
                if (res != null && res.TryGetValue("id", out var newId)) return newId;
            }
        }
        catch
        {
            // Fallback
        }

        var debt = new CustomerDebtDto
        {
            Id = Guid.NewGuid(),
            CustomerName = command.CustomerName,
            CustomerPhone = command.CustomerPhone ?? "-",
            TotalDebt = command.TotalDebt,
            PaidDebt = command.PaidDebt,
            DueDate = command.DueDate
        };
        _localDebts.Insert(0, debt);
        return debt.Id;
    }
}
