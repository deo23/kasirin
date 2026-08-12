namespace KasirIn.Domain.Entities;

public class DebtRecord
{
    private decimal? _remainingDebt;
    private bool? _isSettled;

    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public Tenant? Tenant { get; set; }

    public string CustomerName { get; set; } = string.Empty;
    public string? CustomerPhone { get; set; }

    public decimal TotalDebt { get; set; }
    public decimal PaidDebt { get; set; }

    public decimal RemainingDebt
    {
        get => _remainingDebt ?? (TotalDebt - PaidDebt);
        set => _remainingDebt = value;
    }

    public DateTime? DueDate { get; set; }

    public bool IsSettled
    {
        get => _isSettled ?? (RemainingDebt <= 0);
        set => _isSettled = value;
    }
}
