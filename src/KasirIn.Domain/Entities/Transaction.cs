namespace KasirIn.Domain.Entities;

public class Transaction
{
    private decimal? _totalAmount;
    private decimal? _changeAmount;

    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public Tenant? Tenant { get; set; }

    public Guid UserId { get; set; }
    public User? User { get; set; }

    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

    public decimal TotalAmount
    {
        get => _totalAmount ?? TransactionItems.Sum(item => item.SubTotal);
        set => _totalAmount = value;
    }

    public decimal PaidAmount { get; set; }

    public decimal ChangeAmount
    {
        get => _changeAmount ?? (PaidAmount > TotalAmount ? PaidAmount - TotalAmount : 0);
        set => _changeAmount = value;
    }

    public string PaymentMethod { get; set; } = string.Empty;

    public ICollection<TransactionItem> TransactionItems { get; set; } = new List<TransactionItem>();

    public decimal TotalProfit => TransactionItems.Sum(item => item.Profit);

    public void RecalculateTotals()
    {
        _totalAmount = TransactionItems.Sum(item => item.SubTotal);
        _changeAmount = PaidAmount >= TotalAmount ? PaidAmount - TotalAmount : 0;
    }
}
