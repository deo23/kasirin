namespace KasirIn.Application.Transactions.Queries.GetSalesHistory;

public record TransactionItemDto
{
    public Guid Id { get; init; }
    public Guid ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal CostPrice { get; init; }
    public decimal SubTotal { get; init; }
    public decimal Profit { get; init; }
}

public record TransactionDto
{
    public Guid Id { get; init; }
    public Guid TenantId { get; init; }
    public Guid UserId { get; init; }
    public string UserName { get; init; } = string.Empty;
    public string InvoiceNumber { get; init; } = string.Empty;
    public DateTime TransactionDate { get; init; }
    public decimal TotalAmount { get; init; }
    public decimal PaidAmount { get; init; }
    public decimal ChangeAmount { get; init; }
    public string PaymentMethod { get; init; } = string.Empty;
    public decimal TotalProfit { get; init; }
    public List<TransactionItemDto> Items { get; init; } = new();
}
