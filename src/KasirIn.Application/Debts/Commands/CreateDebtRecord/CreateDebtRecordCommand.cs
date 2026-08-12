namespace KasirIn.Application.Debts.Commands.CreateDebtRecord;

using MediatR;

public record CreateDebtRecordCommand : IRequest<Guid>
{
    public Guid TenantId { get; init; }
    public string CustomerName { get; init; } = string.Empty;
    public string? CustomerPhone { get; init; }
    public decimal TotalDebt { get; init; }
    public decimal PaidDebt { get; init; }
    public DateTime? DueDate { get; init; }
}
