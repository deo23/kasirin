namespace KasirIn.Domain.Entities;

public class TransactionItem
{
    private decimal? _subTotal;
    private decimal? _profit;

    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TransactionId { get; set; }
    public Transaction? Transaction { get; set; }

    public Guid ProductId { get; set; }
    public Product? Product { get; set; }

    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal CostPrice { get; set; }

    public decimal SubTotal
    {
        get => _subTotal ?? (Quantity * UnitPrice);
        set => _subTotal = value;
    }

    public decimal Profit
    {
        get => _profit ?? ((UnitPrice - CostPrice) * Quantity);
        set => _profit = value;
    }
}
