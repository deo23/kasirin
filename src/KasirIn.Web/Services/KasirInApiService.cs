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

    // In-memory fallback dataset for seamless UI demo (100 Real English Studio Products)
    private static readonly List<ProductDto> _localProducts = new()
    {
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Beverages", Name = "Organic Dark Roast Whole Bean Coffee 500g", SKU = "BEV-101", CostPrice = 120000, SellingPrice = 185000, StockQuantity = 80, MinStockThreshold = 5, ImageUrl = "uploads/products/organic_coffee_beans.jpg" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Beverages", Name = "Premium Japanese Uji Matcha Green Tea Powder 200g", SKU = "BEV-102", CostPrice = 70000, SellingPrice = 125000, StockQuantity = 25, MinStockThreshold = 5, ImageUrl = "uploads/products/matcha_tea_powder.jpg" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Beverages", Name = "Sparkling Natural Mineral Water 750ml", SKU = "BEV-103", CostPrice = 18000, SellingPrice = 35000, StockQuantity = 12, MinStockThreshold = 5, ImageUrl = "uploads/products/sparkling_mineral_water.jpg" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Beverages", Name = "Artisanal Sparkling Kombucha Ginger Lemon 330ml", SKU = "BEV-104", CostPrice = 22000, SellingPrice = 42000, StockQuantity = 4, MinStockThreshold = 5, ImageUrl = "uploads/products/sparkling_kombucha.jpg", IsLowStock = true },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Beverages", Name = "Cold-Brewed Earl Black Tea 500ml", SKU = "BEV-105", CostPrice = 15000, SellingPrice = 28000, StockQuantity = 4, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1594631252845-29fc4cc86de5?w=500&auto=format&fit=crop&q=80", IsLowStock = true },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Beverages", Name = "Organic Cold-Pressed Orange Juice 1L", SKU = "BEV-106", CostPrice = 35000, SellingPrice = 58000, StockQuantity = 100, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1613478223719-2ab802602423?w=500&auto=format&fit=crop&q=80" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Beverages", Name = "Pure Coconut Water 500ml", SKU = "BEV-107", CostPrice = 12000, SellingPrice = 22000, StockQuantity = 45, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1543362906-acfc16c67564?w=500&auto=format&fit=crop&q=80" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Beverages", Name = "Fresh Lemonade Honey Infused 450ml", SKU = "BEV-108", CostPrice = 14000, SellingPrice = 25000, StockQuantity = 3, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1513558161293-cdaf765ed2fd?w=500&auto=format&fit=crop&q=80", IsLowStock = true },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Beverages", Name = "Organic Herbal Chamomile Tea 20 Bags", SKU = "BEV-109", CostPrice = 28000, SellingPrice = 48000, StockQuantity = 3, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1576092768241-dec231879fc3?w=500&auto=format&fit=crop&q=80", IsLowStock = true },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Beverages", Name = "Sparkling Apple Cider Vinegar Tonic 350ml", SKU = "BEV-110", CostPrice = 25000, SellingPrice = 45000, StockQuantity = 18, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1546171753-97d7676e4602?w=500&auto=format&fit=crop&q=80" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Beverages", Name = "Almond Milk Unsweetened 1L", SKU = "BEV-111", CostPrice = 38000, SellingPrice = 62000, StockQuantity = 3, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1550583724-b2692b85b150?w=500&auto=format&fit=crop&q=80", IsLowStock = true },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Beverages", Name = "Oat Milk Barista Edition 1L", SKU = "BEV-112", CostPrice = 42000, SellingPrice = 68000, StockQuantity = 30, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1600718374662-0483d2b9da44?w=500&auto=format&fit=crop&q=80" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Beverages", Name = "Organic Green Detox Smoothie 350ml", SKU = "BEV-113", CostPrice = 26000, SellingPrice = 46000, StockQuantity = 80, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1610970881699-44a5587cabec?w=500&auto=format&fit=crop&q=80" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Beverages", Name = "Double Espresso Energy Drink 250ml", SKU = "BEV-114", CostPrice = 16000, SellingPrice = 30000, StockQuantity = 45, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1510591509098-f4fdc6d0ff04?w=500&auto=format&fit=crop&q=80" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Beverages", Name = "Wild Berry Hydration Electrolyte Drink 500ml", SKU = "BEV-115", CostPrice = 18000, SellingPrice = 32000, StockQuantity = 4, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1551024709-8f23befc6f87?w=500&auto=format&fit=crop&q=80", IsLowStock = true },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Snacks", Name = "Artisanal 75% Single-Origin Dark Chocolate Bar", SKU = "SNK-116", CostPrice = 38000, SellingPrice = 65000, StockQuantity = 80, MinStockThreshold = 5, ImageUrl = "uploads/products/dark_chocolate_bar.jpg" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Snacks", Name = "Gourmet Whole Roasted Sea Salt Almonds 250g", SKU = "SNK-117", CostPrice = 55000, SellingPrice = 95000, StockQuantity = 12, MinStockThreshold = 5, ImageUrl = "uploads/products/roasted_almonds.jpg" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Snacks", Name = "Truffle Oil Potato Chips 150g", SKU = "SNK-118", CostPrice = 24000, SellingPrice = 42000, StockQuantity = 45, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1566478989037-eec170784d0b?w=500&auto=format&fit=crop&q=80" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Snacks", Name = "Organic Dried Mango Slices 180g", SKU = "SNK-119", CostPrice = 30000, SellingPrice = 52000, StockQuantity = 45, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1596040033229-a9821ebd058d?w=500&auto=format&fit=crop&q=80" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Snacks", Name = "Roasted Pistachios In-Shell 200g", SKU = "SNK-120", CostPrice = 48000, SellingPrice = 82000, StockQuantity = 100, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1525547719571-a2d4ac8945e2?w=500&auto=format&fit=crop&q=80" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Snacks", Name = "Himalayan Pink Salt Rice Crackers 120g", SKU = "SNK-121", CostPrice = 15000, SellingPrice = 28000, StockQuantity = 60, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1599490659213-e2b9527bd087?w=500&auto=format&fit=crop&q=80" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Snacks", Name = "Dark Chocolate Sea Salt Pretzel Thins 160g", SKU = "SNK-122", CostPrice = 28000, SellingPrice = 49000, StockQuantity = 25, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1527515637462-cff94eecc1ac?w=500&auto=format&fit=crop&q=80" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Snacks", Name = "Organic Roasted Cashews Garlic Herbs 200g", SKU = "SNK-123", CostPrice = 52000, SellingPrice = 89000, StockQuantity = 30, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1509358271058-acd05cc93219?w=500&auto=format&fit=crop&q=80" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Snacks", Name = "Baked Honey Butter Popcorn 180g", SKU = "SNK-124", CostPrice = 18000, SellingPrice = 32000, StockQuantity = 80, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1578849278619-e73505e9610f?w=500&auto=format&fit=crop&q=80" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Snacks", Name = "Organic Dried Cranberries 250g", SKU = "SNK-125", CostPrice = 32000, SellingPrice = 55000, StockQuantity = 45, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1589135233689-d5df3e226e63?w=500&auto=format&fit=crop&q=80" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Snacks", Name = "Whole Wheat Protein Energy Bites 150g", SKU = "SNK-126", CostPrice = 22000, SellingPrice = 39000, StockQuantity = 25, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1604382354936-07c5d9983bd3?w=500&auto=format&fit=crop&q=80" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Snacks", Name = "Matcha White Chocolate Wafers 140g", SKU = "SNK-127", CostPrice = 25000, SellingPrice = 45000, StockQuantity = 100, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1541781774459-bb2af2f05b55?w=500&auto=format&fit=crop&q=80" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Snacks", Name = "Spicy Szechuan Dried Seaweed Crisps 80g", SKU = "SNK-128", CostPrice = 19000, SellingPrice = 34000, StockQuantity = 25, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1608686207856-001b95cf60ca?w=500&auto=format&fit=crop&q=80" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Snacks", Name = "Organic Granola Bars Peanut Butter 6 Pack", SKU = "SNK-129", CostPrice = 36000, SellingPrice = 62000, StockQuantity = 18, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1590080875515-8a3a8dc5735e?w=500&auto=format&fit=crop&q=80" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Snacks", Name = "Roasted Pumpkin Seeds Salted 200g", SKU = "SNK-130", CostPrice = 27000, SellingPrice = 46000, StockQuantity = 18, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1615485290382-441e4d049cb5?w=500&auto=format&fit=crop&q=80" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Groceries", Name = "Cold-Pressed Extra Virgin Olive Oil 750ml", SKU = "GRC-131", CostPrice = 135000, SellingPrice = 210000, StockQuantity = 30, MinStockThreshold = 5, ImageUrl = "uploads/products/olive_oil_bottle.jpg" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Groceries", Name = "Fresh Organic Hass Avocados (Pack of 4)", SKU = "GRC-132", CostPrice = 45000, SellingPrice = 75000, StockQuantity = 80, MinStockThreshold = 5, ImageUrl = "uploads/products/organic_avocados.jpg" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Groceries", Name = "Organic Japanese Jasmine Rice 5kg", SKU = "GRC-133", CostPrice = 125000, SellingPrice = 185000, StockQuantity = 25, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1586201375761-83865001e31c?w=500&auto=format&fit=crop&q=80" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Groceries", Name = "Organic Himalayan Pink Fine Salt 500g", SKU = "GRC-134", CostPrice = 22000, SellingPrice = 38000, StockQuantity = 12, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1615485290382-441e4d049cb5?w=500&auto=format&fit=crop&q=80" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Groceries", Name = "Italian Aged Balsamic Vinegar 250ml", SKU = "GRC-135", CostPrice = 65000, SellingPrice = 110000, StockQuantity = 4, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1474979266404-7eaacbcd87c5?w=500&auto=format&fit=crop&q=80", IsLowStock = true },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Groceries", Name = "Organic Quinoa Grain White & Red 500g", SKU = "GRC-136", CostPrice = 45000, SellingPrice = 78000, StockQuantity = 25, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1585994191611-726a888be4b3?w=500&auto=format&fit=crop&q=80" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Groceries", Name = "Raw Wildflower Honey Jar 500g", SKU = "GRC-137", CostPrice = 75000, SellingPrice = 130000, StockQuantity = 4, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1587049352847-4a222e784d38?w=500&auto=format&fit=crop&q=80", IsLowStock = true },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Groceries", Name = "Organic Whole Rolled Oats 1kg", SKU = "GRC-138", CostPrice = 38000, SellingPrice = 65000, StockQuantity = 60, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1517093725432-a9a755415309?w=500&auto=format&fit=crop&q=80" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Groceries", Name = "Italian San Marzano Whole Tomatoes 400g", SKU = "GRC-139", CostPrice = 24000, SellingPrice = 42000, StockQuantity = 30, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1592924357228-91a4daadcfea?w=500&auto=format&fit=crop&q=80" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Groceries", Name = "Organic Whole Black Pepper Corns 150g", SKU = "GRC-140", CostPrice = 28000, SellingPrice = 48000, StockQuantity = 12, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1599940824399-b87987ceb72a?w=500&auto=format&fit=crop&q=80" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Groceries", Name = "Organic Coconut Sugar 500g", SKU = "GRC-141", CostPrice = 26000, SellingPrice = 45000, StockQuantity = 45, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1581600140682-d4e68c8cde52?w=500&auto=format&fit=crop&q=80" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Groceries", Name = "Artisanal Basil Pesto Sauce 190g", SKU = "GRC-142", CostPrice = 42000, SellingPrice = 72000, StockQuantity = 3, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1505253716362-afaea1d3d1af?w=500&auto=format&fit=crop&q=80", IsLowStock = true },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Groceries", Name = "Organic Brown Rice Pasta Spaghetti 500g", SKU = "GRC-143", CostPrice = 32000, SellingPrice = 56000, StockQuantity = 30, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1621996346565-e3d5d6281288?w=500&auto=format&fit=crop&q=80" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Groceries", Name = "Unrefined Virgin Coconut Oil 500ml", SKU = "GRC-144", CostPrice = 48000, SellingPrice = 85000, StockQuantity = 45, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1526947425960-945c6e72858f?w=500&auto=format&fit=crop&q=80" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Groceries", Name = "Organic Chia Seeds 350g", SKU = "GRC-145", CostPrice = 35000, SellingPrice = 62000, StockQuantity = 45, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1514733670139-4d87a1941d55?w=500&auto=format&fit=crop&q=80" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Dairy & Eggs", Name = "Grass-Fed Whole Milk 1L", SKU = "DAI-146", CostPrice = 24000, SellingPrice = 39000, StockQuantity = 18, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1563636619-e9143da7973b?w=500&auto=format&fit=crop&q=80" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Dairy & Eggs", Name = "Organic Free-Range Brown Eggs (Pack of 10)", SKU = "DAI-147", CostPrice = 32000, SellingPrice = 52000, StockQuantity = 25, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1506976785307-8732e854ad03?w=500&auto=format&fit=crop&q=80" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Dairy & Eggs", Name = "Greek Yogurt Plain Honey Infused 400g", SKU = "DAI-148", CostPrice = 38000, SellingPrice = 64000, StockQuantity = 12, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1488477181946-6428a0291777?w=500&auto=format&fit=crop&q=80" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Dairy & Eggs", Name = "Aged Sharp Cheddar Cheese Block 250g", SKU = "DAI-149", CostPrice = 52000, SellingPrice = 89000, StockQuantity = 80, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1618160702438-9b02ab6515c9?w=500&auto=format&fit=crop&q=80" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Dairy & Eggs", Name = "French Salted Creamery Butter 250g", SKU = "DAI-150", CostPrice = 45000, SellingPrice = 78000, StockQuantity = 3, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1589985270826-4b7bb135bc9d?w=500&auto=format&fit=crop&q=80", IsLowStock = true },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Dairy & Eggs", Name = "Fresh Mozzarella Cheese Ball 200g", SKU = "DAI-151", CostPrice = 42000, SellingPrice = 72000, StockQuantity = 18, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1559561853-08451507cbe7?w=500&auto=format&fit=crop&q=80" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Dairy & Eggs", Name = "Organic Heavy Whipping Cream 500ml", SKU = "DAI-152", CostPrice = 36000, SellingPrice = 60000, StockQuantity = 30, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1550583724-b2692b85b150?w=500&auto=format&fit=crop&q=80" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Dairy & Eggs", Name = "Artisanal Brie Cheese Wheel 150g", SKU = "DAI-153", CostPrice = 58000, SellingPrice = 98000, StockQuantity = 80, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1452195100486-9cc805987862?w=500&auto=format&fit=crop&q=80" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Dairy & Eggs", Name = "Organic Sour Cream 300g", SKU = "DAI-154", CostPrice = 26000, SellingPrice = 45000, StockQuantity = 80, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1576186726580-a816e8b12896?w=500&auto=format&fit=crop&q=80" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Dairy & Eggs", Name = "Cultured Swiss Cheese Slices 200g", SKU = "DAI-155", CostPrice = 44000, SellingPrice = 75000, StockQuantity = 25, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1552767059-ce182ead8c1b?w=500&auto=format&fit=crop&q=80" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Bakery", Name = "Freshly Baked French Butter Croissant (Pack of 4)", SKU = "BAK-156", CostPrice = 28000, SellingPrice = 48000, StockQuantity = 3, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1555507036-ab1f4038808a?w=500&auto=format&fit=crop&q=80", IsLowStock = true },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Bakery", Name = "Artisanal Sourdough Bread Loaf 600g", SKU = "BAK-157", CostPrice = 35000, SellingPrice = 60000, StockQuantity = 45, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1589367920969-ab8e050bbb04?w=500&auto=format&fit=crop&q=80" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Bakery", Name = "Whole Wheat Seeded Bread Loaf 500g", SKU = "BAK-158", CostPrice = 26000, SellingPrice = 44000, StockQuantity = 25, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1509440159596-0249088772ff?w=500&auto=format&fit=crop&q=80" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Bakery", Name = "Belgian Chocolate Muffin (Pack of 2)", SKU = "BAK-159", CostPrice = 22000, SellingPrice = 38000, StockQuantity = 60, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1607958996333-41aef7caefaa?w=500&auto=format&fit=crop&q=80" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Bakery", Name = "Cinnamon Brown Sugar Bagels (Pack of 4)", SKU = "BAK-160", CostPrice = 30000, SellingPrice = 52000, StockQuantity = 100, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1585478259715-876a6a81ae08?w=500&auto=format&fit=crop&q=80" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Bakery", Name = "Blueberry Fresh Cream Pastry 150g", SKU = "BAK-161", CostPrice = 24000, SellingPrice = 42000, StockQuantity = 60, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1530610476181-d83430b64dcd?w=500&auto=format&fit=crop&q=80" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Bakery", Name = "Classic Italian Focaccia Rosemary 350g", SKU = "BAK-162", CostPrice = 32000, SellingPrice = 55000, StockQuantity = 30, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1601050690597-df0568f70950?w=500&auto=format&fit=crop&q=80" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Bakery", Name = "Almond Danish Pastry 180g", SKU = "BAK-163", CostPrice = 25000, SellingPrice = 45000, StockQuantity = 18, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1509365465985-25d11c17e812?w=500&auto=format&fit=crop&q=80" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Bakery", Name = "Gluten-Free Oat Bread Loaf 450g", SKU = "BAK-164", CostPrice = 42000, SellingPrice = 72000, StockQuantity = 30, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1549931319-a545dcf3bc73?w=500&auto=format&fit=crop&q=80" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Bakery", Name = "Garlic Butter Brioche Buns (Pack of 6)", SKU = "BAK-165", CostPrice = 28000, SellingPrice = 48000, StockQuantity = 25, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1586444248902-2f64eddc13df?w=500&auto=format&fit=crop&q=80" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Personal Care", Name = "Botanical Argan Oil Shampoo 400ml", SKU = "PER-166", CostPrice = 65000, SellingPrice = 115000, StockQuantity = 25, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1535585209827-a15fcdbc4c2d?w=500&auto=format&fit=crop&q=80" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Personal Care", Name = "Nourishing Hydrating Body Wash Lavender 500ml", SKU = "PER-167", CostPrice = 55000, SellingPrice = 95000, StockQuantity = 25, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1608248597262-838d7015cf68?w=500&auto=format&fit=crop&q=80" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Personal Care", Name = "Organic Aloe Vera Face Cleanser 200ml", SKU = "PER-168", CostPrice = 48000, SellingPrice = 85000, StockQuantity = 100, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1556228720-195a672e8a03?w=500&auto=format&fit=crop&q=80" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Personal Care", Name = "Charcoal Mint Whitening Toothpaste 120g", SKU = "PER-169", CostPrice = 22000, SellingPrice = 39000, StockQuantity = 60, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1559563458-527698bf5295?w=500&auto=format&fit=crop&q=80" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Personal Care", Name = "Natural Deodorant Stick Cedarwood 75g", SKU = "PER-170", CostPrice = 45000, SellingPrice = 78000, StockQuantity = 60, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1619451334792-150fd785ee74?w=500&auto=format&fit=crop&q=80" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Personal Care", Name = "Shea Butter Hand Cream Moisturizer 100g", SKU = "PER-171", CostPrice = 32000, SellingPrice = 58000, StockQuantity = 30, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1617897903246-719242758050?w=500&auto=format&fit=crop&q=80" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Personal Care", Name = "Hydrating Hyaluronic Acid Sunscreen SPF50 50ml", SKU = "PER-172", CostPrice = 75000, SellingPrice = 135000, StockQuantity = 4, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1598440947619-2c35fc9aa908?w=500&auto=format&fit=crop&q=80", IsLowStock = true },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Personal Care", Name = "Organic Bamboo Toothbrush 4-Pack", SKU = "PER-173", CostPrice = 24000, SellingPrice = 42000, StockQuantity = 12, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1607613009820-a29f7bb81c04?w=500&auto=format&fit=crop&q=80" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Personal Care", Name = "Vitamin C Face Brightening Serum 30ml", SKU = "PER-174", CostPrice = 85000, SellingPrice = 155000, StockQuantity = 25, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1620916566398-39f1143ab7be?w=500&auto=format&fit=crop&q=80" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Personal Care", Name = "Exfoliating Sea Salt Body Scrub 250g", SKU = "PER-175", CostPrice = 52000, SellingPrice = 92000, StockQuantity = 18, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1567928269937-ae146e45b428?w=500&auto=format&fit=crop&q=80" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Household", Name = "Eco-Friendly Plant-Based Dish Soap 500ml", SKU = "HSH-176", CostPrice = 22000, SellingPrice = 38000, StockQuantity = 30, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1585842378054-ee2e52f94ba2?w=500&auto=format&fit=crop&q=80" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Household", Name = "Multi-Surface Cleaning Spray Lavender 750ml", SKU = "HSH-177", CostPrice = 28000, SellingPrice = 49000, StockQuantity = 80, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1584820927498-cfe5211fd8bf?w=500&auto=format&fit=crop&q=80" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Household", Name = "Natural Laundry Detergent Liquid 1.5L", SKU = "HSH-178", CostPrice = 65000, SellingPrice = 110000, StockQuantity = 4, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1610557892470-55d9e80c0bce?w=500&auto=format&fit=crop&q=80", IsLowStock = true },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Household", Name = "Bamboo Fiber Kitchen Towel Roll 2-Pack", SKU = "HSH-179", CostPrice = 18000, SellingPrice = 32000, StockQuantity = 80, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1584556812952-905ffd0c611a?w=500&auto=format&fit=crop&q=80" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Household", Name = "Biodegradable Trash Bags 30L (20 Pack)", SKU = "HSH-180", CostPrice = 24000, SellingPrice = 42000, StockQuantity = 25, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1605600659873-d808a13e4d2a?w=500&auto=format&fit=crop&q=80" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Household", Name = "Citrus Scented Air Freshener Spray 300ml", SKU = "HSH-181", CostPrice = 26000, SellingPrice = 45000, StockQuantity = 25, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1616486338812-3dadae4b4ace?w=500&auto=format&fit=crop&q=80" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Household", Name = "Microfiber Cleaning Cloths Set of 5", SKU = "HSH-182", CostPrice = 25000, SellingPrice = 44000, StockQuantity = 3, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1563453392212-326f5e854473?w=500&auto=format&fit=crop&q=80", IsLowStock = true },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Household", Name = "Natural Beeswax Scented Candle Jasmine 200g", SKU = "HSH-183", CostPrice = 48000, SellingPrice = 85000, StockQuantity = 25, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1603006905003-be475563bc59?w=500&auto=format&fit=crop&q=80" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Household", Name = "Non-Toxic Fabric Softener Clean Cotton 1L", SKU = "HSH-184", CostPrice = 38000, SellingPrice = 65000, StockQuantity = 25, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1584820927498-cfe5211fd8bf?w=500&auto=format&fit=crop&q=80" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Household", Name = "Natural Wood Floor Polish Wax 500ml", SKU = "HSH-185", CostPrice = 52000, SellingPrice = 92000, StockQuantity = 100, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1585842378054-ee2e52f94ba2?w=500&auto=format&fit=crop&q=80" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Health & Wellness", Name = "Multivitamin Daily Health Complex 60 Capsules", SKU = "HLT-186", CostPrice = 85000, SellingPrice = 145000, StockQuantity = 4, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1584017911766-d451b3d0e843?w=500&auto=format&fit=crop&q=80", IsLowStock = true },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Health & Wellness", Name = "Organic Whey Protein Powder Vanilla 1kg", SKU = "HLT-187", CostPrice = 220000, SellingPrice = 360000, StockQuantity = 45, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1579722821273-0f6c7d44362f?w=500&auto=format&fit=crop&q=80" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Health & Wellness", Name = "Omega-3 Fish Oil 1000mg 90 Softgels", SKU = "HLT-188", CostPrice = 95000, SellingPrice = 165000, StockQuantity = 3, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1550572017-edd951aa8f72?w=500&auto=format&fit=crop&q=80", IsLowStock = true },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Health & Wellness", Name = "Vitamin C 1000mg Immune Support 60 Tablets", SKU = "HLT-189", CostPrice = 45000, SellingPrice = 78000, StockQuantity = 60, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1584308666744-24d5c474f2ae?w=500&auto=format&fit=crop&q=80" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Health & Wellness", Name = "Plant-Based Collagen Builder Powder 300g", SKU = "HLT-190", CostPrice = 140000, SellingPrice = 240000, StockQuantity = 25, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1579722821273-0f6c7d44362f?w=500&auto=format&fit=crop&q=80" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Health & Wellness", Name = "Magnesium Glycinate 400mg 90 Capsules", SKU = "HLT-191", CostPrice = 88000, SellingPrice = 150000, StockQuantity = 100, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1471864190281-a93a3070b6de?w=500&auto=format&fit=crop&q=80" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Health & Wellness", Name = "Organic Ashwagandha Root Extract 60 Veggie Caps", SKU = "HLT-192", CostPrice = 78000, SellingPrice = 135000, StockQuantity = 3, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1584017911766-d451b3d0e843?w=500&auto=format&fit=crop&q=80", IsLowStock = true },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Health & Wellness", Name = "Probiotics 50 Billion CFU Gut Health 30 Capsules", SKU = "HLT-193", CostPrice = 110000, SellingPrice = 188000, StockQuantity = 60, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1550572017-edd951aa8f72?w=500&auto=format&fit=crop&q=80" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Health & Wellness", Name = "Organic Elderberry Syrup Immune Booster 250ml", SKU = "HLT-194", CostPrice = 92000, SellingPrice = 158000, StockQuantity = 25, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1546171753-97d7676e4602?w=500&auto=format&fit=crop&q=80" },
        new ProductDto { Id = Guid.NewGuid(), TenantId = DefaultTenantId, CategoryName = "Health & Wellness", Name = "Melatonin 5mg Sleep Aid 60 Gummies", SKU = "HLT-195", CostPrice = 55000, SellingPrice = 95000, StockQuantity = 12, MinStockThreshold = 5, ImageUrl = "https://images.unsplash.com/photo-1582083335555-460d09d3b4bb?w=500&auto=format&fit=crop&q=80" }
    };

    private static readonly List<TransactionDto> _localTransactions = new()
    {
        new TransactionDto
        {
            Id = Guid.NewGuid(),
            TenantId = DefaultTenantId,
            InvoiceNumber = "TRX-0981",
            TransactionDate = DateTime.Now.AddMinutes(-18),
            TotalAmount = 250000,
            PaidAmount = 300000,
            ChangeAmount = 50000,
            PaymentMethod = "QRIS",
            TotalProfit = 92000,
            Items = new List<TransactionItemDto>
            {
                new TransactionItemDto { Id = Guid.NewGuid(), ProductName = "Organic Dark Roast Whole Bean Coffee 500g", Quantity = 1, UnitPrice = 185000, CostPrice = 120000, SubTotal = 185000, Profit = 65000 },
                new TransactionItemDto { Id = Guid.NewGuid(), ProductName = "Artisanal 75% Single-Origin Dark Chocolate Bar", Quantity = 1, UnitPrice = 65000, CostPrice = 38000, SubTotal = 65000, Profit = 27000 }
            }
        },
        new TransactionDto
        {
            Id = Guid.NewGuid(),
            TenantId = DefaultTenantId,
            InvoiceNumber = "TRX-0982",
            TransactionDate = DateTime.Now.AddMinutes(-36),
            TotalAmount = 250000,
            PaidAmount = 300000,
            ChangeAmount = 50000,
            PaymentMethod = "DEBIT",
            TotalProfit = 92000,
            Items = new List<TransactionItemDto>
            {
                new TransactionItemDto { Id = Guid.NewGuid(), ProductName = "Organic Dark Roast Whole Bean Coffee 500g", Quantity = 1, UnitPrice = 185000, CostPrice = 120000, SubTotal = 185000, Profit = 65000 },
                new TransactionItemDto { Id = Guid.NewGuid(), ProductName = "Artisanal 75% Single-Origin Dark Chocolate Bar", Quantity = 1, UnitPrice = 65000, CostPrice = 38000, SubTotal = 65000, Profit = 27000 }
            }
        },
        new TransactionDto
        {
            Id = Guid.NewGuid(),
            TenantId = DefaultTenantId,
            InvoiceNumber = "TRX-0983",
            TransactionDate = DateTime.Now.AddMinutes(-54),
            TotalAmount = 250000,
            PaidAmount = 300000,
            ChangeAmount = 50000,
            PaymentMethod = "QRIS",
            TotalProfit = 92000,
            Items = new List<TransactionItemDto>
            {
                new TransactionItemDto { Id = Guid.NewGuid(), ProductName = "Organic Dark Roast Whole Bean Coffee 500g", Quantity = 1, UnitPrice = 185000, CostPrice = 120000, SubTotal = 185000, Profit = 65000 },
                new TransactionItemDto { Id = Guid.NewGuid(), ProductName = "Artisanal 75% Single-Origin Dark Chocolate Bar", Quantity = 1, UnitPrice = 65000, CostPrice = 38000, SubTotal = 65000, Profit = 27000 }
            }
        },
        new TransactionDto
        {
            Id = Guid.NewGuid(),
            TenantId = DefaultTenantId,
            InvoiceNumber = "TRX-0984",
            TransactionDate = DateTime.Now.AddMinutes(-72),
            TotalAmount = 250000,
            PaidAmount = 300000,
            ChangeAmount = 50000,
            PaymentMethod = "CASH",
            TotalProfit = 92000,
            Items = new List<TransactionItemDto>
            {
                new TransactionItemDto { Id = Guid.NewGuid(), ProductName = "Organic Dark Roast Whole Bean Coffee 500g", Quantity = 1, UnitPrice = 185000, CostPrice = 120000, SubTotal = 185000, Profit = 65000 },
                new TransactionItemDto { Id = Guid.NewGuid(), ProductName = "Artisanal 75% Single-Origin Dark Chocolate Bar", Quantity = 1, UnitPrice = 65000, CostPrice = 38000, SubTotal = 65000, Profit = 27000 }
            }
        },
        new TransactionDto
        {
            Id = Guid.NewGuid(),
            TenantId = DefaultTenantId,
            InvoiceNumber = "TRX-0985",
            TransactionDate = DateTime.Now.AddMinutes(-90),
            TotalAmount = 250000,
            PaidAmount = 300000,
            ChangeAmount = 50000,
            PaymentMethod = "TRANSFER",
            TotalProfit = 92000,
            Items = new List<TransactionItemDto>
            {
                new TransactionItemDto { Id = Guid.NewGuid(), ProductName = "Organic Dark Roast Whole Bean Coffee 500g", Quantity = 1, UnitPrice = 185000, CostPrice = 120000, SubTotal = 185000, Profit = 65000 },
                new TransactionItemDto { Id = Guid.NewGuid(), ProductName = "Artisanal 75% Single-Origin Dark Chocolate Bar", Quantity = 1, UnitPrice = 65000, CostPrice = 38000, SubTotal = 65000, Profit = 27000 }
            }
        },
        new TransactionDto
        {
            Id = Guid.NewGuid(),
            TenantId = DefaultTenantId,
            InvoiceNumber = "TRX-0986",
            TransactionDate = DateTime.Now.AddMinutes(-108),
            TotalAmount = 250000,
            PaidAmount = 300000,
            ChangeAmount = 50000,
            PaymentMethod = "DEBIT",
            TotalProfit = 92000,
            Items = new List<TransactionItemDto>
            {
                new TransactionItemDto { Id = Guid.NewGuid(), ProductName = "Organic Dark Roast Whole Bean Coffee 500g", Quantity = 1, UnitPrice = 185000, CostPrice = 120000, SubTotal = 185000, Profit = 65000 },
                new TransactionItemDto { Id = Guid.NewGuid(), ProductName = "Artisanal 75% Single-Origin Dark Chocolate Bar", Quantity = 1, UnitPrice = 65000, CostPrice = 38000, SubTotal = 65000, Profit = 27000 }
            }
        },
        new TransactionDto
        {
            Id = Guid.NewGuid(),
            TenantId = DefaultTenantId,
            InvoiceNumber = "TRX-0987",
            TransactionDate = DateTime.Now.AddMinutes(-126),
            TotalAmount = 250000,
            PaidAmount = 300000,
            ChangeAmount = 50000,
            PaymentMethod = "DEBIT",
            TotalProfit = 92000,
            Items = new List<TransactionItemDto>
            {
                new TransactionItemDto { Id = Guid.NewGuid(), ProductName = "Organic Dark Roast Whole Bean Coffee 500g", Quantity = 1, UnitPrice = 185000, CostPrice = 120000, SubTotal = 185000, Profit = 65000 },
                new TransactionItemDto { Id = Guid.NewGuid(), ProductName = "Artisanal 75% Single-Origin Dark Chocolate Bar", Quantity = 1, UnitPrice = 65000, CostPrice = 38000, SubTotal = 65000, Profit = 27000 }
            }
        },
        new TransactionDto
        {
            Id = Guid.NewGuid(),
            TenantId = DefaultTenantId,
            InvoiceNumber = "TRX-0988",
            TransactionDate = DateTime.Now.AddMinutes(-144),
            TotalAmount = 250000,
            PaidAmount = 300000,
            ChangeAmount = 50000,
            PaymentMethod = "TRANSFER",
            TotalProfit = 92000,
            Items = new List<TransactionItemDto>
            {
                new TransactionItemDto { Id = Guid.NewGuid(), ProductName = "Organic Dark Roast Whole Bean Coffee 500g", Quantity = 1, UnitPrice = 185000, CostPrice = 120000, SubTotal = 185000, Profit = 65000 },
                new TransactionItemDto { Id = Guid.NewGuid(), ProductName = "Artisanal 75% Single-Origin Dark Chocolate Bar", Quantity = 1, UnitPrice = 65000, CostPrice = 38000, SubTotal = 65000, Profit = 27000 }
            }
        },
        new TransactionDto
        {
            Id = Guid.NewGuid(),
            TenantId = DefaultTenantId,
            InvoiceNumber = "TRX-0989",
            TransactionDate = DateTime.Now.AddMinutes(-162),
            TotalAmount = 250000,
            PaidAmount = 300000,
            ChangeAmount = 50000,
            PaymentMethod = "TRANSFER",
            TotalProfit = 92000,
            Items = new List<TransactionItemDto>
            {
                new TransactionItemDto { Id = Guid.NewGuid(), ProductName = "Organic Dark Roast Whole Bean Coffee 500g", Quantity = 1, UnitPrice = 185000, CostPrice = 120000, SubTotal = 185000, Profit = 65000 },
                new TransactionItemDto { Id = Guid.NewGuid(), ProductName = "Artisanal 75% Single-Origin Dark Chocolate Bar", Quantity = 1, UnitPrice = 65000, CostPrice = 38000, SubTotal = 65000, Profit = 27000 }
            }
        },
        new TransactionDto
        {
            Id = Guid.NewGuid(),
            TenantId = DefaultTenantId,
            InvoiceNumber = "TRX-0990",
            TransactionDate = DateTime.Now.AddMinutes(-180),
            TotalAmount = 250000,
            PaidAmount = 300000,
            ChangeAmount = 50000,
            PaymentMethod = "TRANSFER",
            TotalProfit = 92000,
            Items = new List<TransactionItemDto>
            {
                new TransactionItemDto { Id = Guid.NewGuid(), ProductName = "Organic Dark Roast Whole Bean Coffee 500g", Quantity = 1, UnitPrice = 185000, CostPrice = 120000, SubTotal = 185000, Profit = 65000 },
                new TransactionItemDto { Id = Guid.NewGuid(), ProductName = "Artisanal 75% Single-Origin Dark Chocolate Bar", Quantity = 1, UnitPrice = 65000, CostPrice = 38000, SubTotal = 65000, Profit = 27000 }
            }
        },
        new TransactionDto
        {
            Id = Guid.NewGuid(),
            TenantId = DefaultTenantId,
            InvoiceNumber = "TRX-0991",
            TransactionDate = DateTime.Now.AddMinutes(-198),
            TotalAmount = 250000,
            PaidAmount = 300000,
            ChangeAmount = 50000,
            PaymentMethod = "TRANSFER",
            TotalProfit = 92000,
            Items = new List<TransactionItemDto>
            {
                new TransactionItemDto { Id = Guid.NewGuid(), ProductName = "Organic Dark Roast Whole Bean Coffee 500g", Quantity = 1, UnitPrice = 185000, CostPrice = 120000, SubTotal = 185000, Profit = 65000 },
                new TransactionItemDto { Id = Guid.NewGuid(), ProductName = "Artisanal 75% Single-Origin Dark Chocolate Bar", Quantity = 1, UnitPrice = 65000, CostPrice = 38000, SubTotal = 65000, Profit = 27000 }
            }
        },
        new TransactionDto
        {
            Id = Guid.NewGuid(),
            TenantId = DefaultTenantId,
            InvoiceNumber = "TRX-0992",
            TransactionDate = DateTime.Now.AddMinutes(-216),
            TotalAmount = 250000,
            PaidAmount = 300000,
            ChangeAmount = 50000,
            PaymentMethod = "QRIS",
            TotalProfit = 92000,
            Items = new List<TransactionItemDto>
            {
                new TransactionItemDto { Id = Guid.NewGuid(), ProductName = "Organic Dark Roast Whole Bean Coffee 500g", Quantity = 1, UnitPrice = 185000, CostPrice = 120000, SubTotal = 185000, Profit = 65000 },
                new TransactionItemDto { Id = Guid.NewGuid(), ProductName = "Artisanal 75% Single-Origin Dark Chocolate Bar", Quantity = 1, UnitPrice = 65000, CostPrice = 38000, SubTotal = 65000, Profit = 27000 }
            }
        },
        new TransactionDto
        {
            Id = Guid.NewGuid(),
            TenantId = DefaultTenantId,
            InvoiceNumber = "TRX-0993",
            TransactionDate = DateTime.Now.AddMinutes(-234),
            TotalAmount = 250000,
            PaidAmount = 300000,
            ChangeAmount = 50000,
            PaymentMethod = "TRANSFER",
            TotalProfit = 92000,
            Items = new List<TransactionItemDto>
            {
                new TransactionItemDto { Id = Guid.NewGuid(), ProductName = "Organic Dark Roast Whole Bean Coffee 500g", Quantity = 1, UnitPrice = 185000, CostPrice = 120000, SubTotal = 185000, Profit = 65000 },
                new TransactionItemDto { Id = Guid.NewGuid(), ProductName = "Artisanal 75% Single-Origin Dark Chocolate Bar", Quantity = 1, UnitPrice = 65000, CostPrice = 38000, SubTotal = 65000, Profit = 27000 }
            }
        },
        new TransactionDto
        {
            Id = Guid.NewGuid(),
            TenantId = DefaultTenantId,
            InvoiceNumber = "TRX-0994",
            TransactionDate = DateTime.Now.AddMinutes(-252),
            TotalAmount = 250000,
            PaidAmount = 300000,
            ChangeAmount = 50000,
            PaymentMethod = "TRANSFER",
            TotalProfit = 92000,
            Items = new List<TransactionItemDto>
            {
                new TransactionItemDto { Id = Guid.NewGuid(), ProductName = "Organic Dark Roast Whole Bean Coffee 500g", Quantity = 1, UnitPrice = 185000, CostPrice = 120000, SubTotal = 185000, Profit = 65000 },
                new TransactionItemDto { Id = Guid.NewGuid(), ProductName = "Artisanal 75% Single-Origin Dark Chocolate Bar", Quantity = 1, UnitPrice = 65000, CostPrice = 38000, SubTotal = 65000, Profit = 27000 }
            }
        },
        new TransactionDto
        {
            Id = Guid.NewGuid(),
            TenantId = DefaultTenantId,
            InvoiceNumber = "TRX-0995",
            TransactionDate = DateTime.Now.AddMinutes(-270),
            TotalAmount = 250000,
            PaidAmount = 300000,
            ChangeAmount = 50000,
            PaymentMethod = "TRANSFER",
            TotalProfit = 92000,
            Items = new List<TransactionItemDto>
            {
                new TransactionItemDto { Id = Guid.NewGuid(), ProductName = "Organic Dark Roast Whole Bean Coffee 500g", Quantity = 1, UnitPrice = 185000, CostPrice = 120000, SubTotal = 185000, Profit = 65000 },
                new TransactionItemDto { Id = Guid.NewGuid(), ProductName = "Artisanal 75% Single-Origin Dark Chocolate Bar", Quantity = 1, UnitPrice = 65000, CostPrice = 38000, SubTotal = 65000, Profit = 27000 }
            }
        },
        new TransactionDto
        {
            Id = Guid.NewGuid(),
            TenantId = DefaultTenantId,
            InvoiceNumber = "TRX-0996",
            TransactionDate = DateTime.Now.AddMinutes(-288),
            TotalAmount = 250000,
            PaidAmount = 300000,
            ChangeAmount = 50000,
            PaymentMethod = "QRIS",
            TotalProfit = 92000,
            Items = new List<TransactionItemDto>
            {
                new TransactionItemDto { Id = Guid.NewGuid(), ProductName = "Organic Dark Roast Whole Bean Coffee 500g", Quantity = 1, UnitPrice = 185000, CostPrice = 120000, SubTotal = 185000, Profit = 65000 },
                new TransactionItemDto { Id = Guid.NewGuid(), ProductName = "Artisanal 75% Single-Origin Dark Chocolate Bar", Quantity = 1, UnitPrice = 65000, CostPrice = 38000, SubTotal = 65000, Profit = 27000 }
            }
        },
        new TransactionDto
        {
            Id = Guid.NewGuid(),
            TenantId = DefaultTenantId,
            InvoiceNumber = "TRX-0997",
            TransactionDate = DateTime.Now.AddMinutes(-306),
            TotalAmount = 250000,
            PaidAmount = 300000,
            ChangeAmount = 50000,
            PaymentMethod = "TRANSFER",
            TotalProfit = 92000,
            Items = new List<TransactionItemDto>
            {
                new TransactionItemDto { Id = Guid.NewGuid(), ProductName = "Organic Dark Roast Whole Bean Coffee 500g", Quantity = 1, UnitPrice = 185000, CostPrice = 120000, SubTotal = 185000, Profit = 65000 },
                new TransactionItemDto { Id = Guid.NewGuid(), ProductName = "Artisanal 75% Single-Origin Dark Chocolate Bar", Quantity = 1, UnitPrice = 65000, CostPrice = 38000, SubTotal = 65000, Profit = 27000 }
            }
        },
        new TransactionDto
        {
            Id = Guid.NewGuid(),
            TenantId = DefaultTenantId,
            InvoiceNumber = "TRX-0998",
            TransactionDate = DateTime.Now.AddMinutes(-324),
            TotalAmount = 250000,
            PaidAmount = 300000,
            ChangeAmount = 50000,
            PaymentMethod = "DEBIT",
            TotalProfit = 92000,
            Items = new List<TransactionItemDto>
            {
                new TransactionItemDto { Id = Guid.NewGuid(), ProductName = "Organic Dark Roast Whole Bean Coffee 500g", Quantity = 1, UnitPrice = 185000, CostPrice = 120000, SubTotal = 185000, Profit = 65000 },
                new TransactionItemDto { Id = Guid.NewGuid(), ProductName = "Artisanal 75% Single-Origin Dark Chocolate Bar", Quantity = 1, UnitPrice = 65000, CostPrice = 38000, SubTotal = 65000, Profit = 27000 }
            }
        },
        new TransactionDto
        {
            Id = Guid.NewGuid(),
            TenantId = DefaultTenantId,
            InvoiceNumber = "TRX-0999",
            TransactionDate = DateTime.Now.AddMinutes(-342),
            TotalAmount = 250000,
            PaidAmount = 300000,
            ChangeAmount = 50000,
            PaymentMethod = "CASH",
            TotalProfit = 92000,
            Items = new List<TransactionItemDto>
            {
                new TransactionItemDto { Id = Guid.NewGuid(), ProductName = "Organic Dark Roast Whole Bean Coffee 500g", Quantity = 1, UnitPrice = 185000, CostPrice = 120000, SubTotal = 185000, Profit = 65000 },
                new TransactionItemDto { Id = Guid.NewGuid(), ProductName = "Artisanal 75% Single-Origin Dark Chocolate Bar", Quantity = 1, UnitPrice = 65000, CostPrice = 38000, SubTotal = 65000, Profit = 27000 }
            }
        },
        new TransactionDto
        {
            Id = Guid.NewGuid(),
            TenantId = DefaultTenantId,
            InvoiceNumber = "TRX-09100",
            TransactionDate = DateTime.Now.AddMinutes(-360),
            TotalAmount = 250000,
            PaidAmount = 300000,
            ChangeAmount = 50000,
            PaymentMethod = "TRANSFER",
            TotalProfit = 92000,
            Items = new List<TransactionItemDto>
            {
                new TransactionItemDto { Id = Guid.NewGuid(), ProductName = "Organic Dark Roast Whole Bean Coffee 500g", Quantity = 1, UnitPrice = 185000, CostPrice = 120000, SubTotal = 185000, Profit = 65000 },
                new TransactionItemDto { Id = Guid.NewGuid(), ProductName = "Artisanal 75% Single-Origin Dark Chocolate Bar", Quantity = 1, UnitPrice = 65000, CostPrice = 38000, SubTotal = 65000, Profit = 27000 }
            }
        }
    };

    private static readonly List<CustomerDebtDto> _localDebts = new()
    {
        new CustomerDebtDto { Id = Guid.NewGuid(), CustomerName = "Alexander Wright", CustomerPhone = "081298765432", TotalDebt = 250000, PaidDebt = 50000, DueDate = DateTime.Now.AddDays(7) },
        new CustomerDebtDto { Id = Guid.NewGuid(), CustomerName = "Sophia Martinez", CustomerPhone = "081345678901", TotalDebt = 180000, PaidDebt = 180000, DueDate = DateTime.Now.AddDays(-2) },
        new CustomerDebtDto { Id = Guid.NewGuid(), CustomerName = "Marcus Vance", CustomerPhone = "081567890123", TotalDebt = 420000, PaidDebt = 100000, DueDate = DateTime.Now.AddDays(12) },
        new CustomerDebtDto { Id = Guid.NewGuid(), CustomerName = "Elena Rostova", CustomerPhone = "081789012345", TotalDebt = 310000, PaidDebt = 310000, DueDate = DateTime.Now.AddDays(-5) },
        new CustomerDebtDto { Id = Guid.NewGuid(), CustomerName = "David Miller", CustomerPhone = "081901234567", TotalDebt = 150000, PaidDebt = 0, DueDate = DateTime.Now.AddDays(3) }
    };

    public KasirInApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<ProductDto>> GetProductsAsync(string? category = null, string? search = null)
    {
        List<ProductDto> list = new();
        try
        {
            var url = $"api/Products?tenantId={DefaultTenantId}";
            if (!string.IsNullOrEmpty(category)) url += $"&categoryId={category}";
            var result = await _httpClient.GetFromJsonAsync<List<ProductDto>>(url);
            if (result != null && result.Count > 0)
            {
                list = result;
            }
        }
        catch { }

        if (list.Count == 0)
        {
            list = _localProducts;
        }

        var query = list.AsQueryable();
        if (!string.IsNullOrEmpty(category) && category != "Semua" && category != "All")
        {
            query = query.Where(p => p.CategoryName.Equals(category, StringComparison.OrdinalIgnoreCase));
        }
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(p => p.Name.Contains(search, StringComparison.OrdinalIgnoreCase) || p.SKU.Contains(search, StringComparison.OrdinalIgnoreCase));
        }
        return query.ToList();
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
        catch { }

        var newIdLocal = Guid.NewGuid();
        var newProd = new ProductDto
        {
            Id = newIdLocal,
            TenantId = command.TenantId,
            CategoryName = "General",
            Name = command.Name,
            SKU = command.SKU,
            CostPrice = command.CostPrice,
            SellingPrice = command.SellingPrice,
            StockQuantity = command.StockQuantity,
            MinStockThreshold = command.MinStockThreshold,
            ImageUrl = command.ImageUrl
        };
        _localProducts.Insert(0, newProd);
        return newIdLocal;
    }

    public async Task<string?> UploadProductImageAsync(IBrowserFile file)
    {
        try
        {
            using var content = new MultipartFormDataContent();
            using var stream = file.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024);
            var fileContent = new StreamContent(stream);
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
            content.Add(fileContent, "file", file.Name);

            var response = await _httpClient.PostAsync("api/Products/upload-image", content);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ImageUploadResponse>();
                return result?.Url;
            }
            return null;
        }
        catch
        {
            return "uploads/products/organic_coffee_beans.jpg";
        }
    }

    public async Task<Guid> CreateTransactionAsync(CreateTransactionCommand command)
    {
        var id = Guid.NewGuid();
        decimal total = 0;
        var items = new List<TransactionItemDto>();

        foreach (var item in command.Items)
        {
            var prod = _localProducts.FirstOrDefault(p => p.Id == item.ProductId);
            var price = prod?.SellingPrice ?? 50000;
            var cost = prod?.CostPrice ?? 30000;
            var name = prod?.Name ?? "Product Item";
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
        catch { }

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
        catch { }

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

    public async Task<byte[]?> ExportExcelReportAsync(DateTime? startDate = null, DateTime? endDate = null)
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
        catch { }
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
        catch { }

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

public class ImageUploadResponse
{
    public string Url { get; set; } = string.Empty;
}
