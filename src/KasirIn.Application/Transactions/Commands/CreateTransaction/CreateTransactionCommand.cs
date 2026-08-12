namespace KasirIn.Application.Transactions.Commands.CreateTransaction;

using MediatR;

public record CreateTransactionItemCommandDto(Guid ProductId, int Quantity);

public record CreateTransactionCommand : IRequest<Guid>
{
    public Guid TenantId { get; init; }
    public Guid UserId { get; init; }
    public string PaymentMethod { get; init; } = "CASH";
    public decimal PaidAmount { get; init; }
    public List<CreateTransactionItemCommandDto> Items { get; init; } = new();
}
