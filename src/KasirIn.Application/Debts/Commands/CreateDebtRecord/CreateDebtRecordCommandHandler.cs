namespace KasirIn.Application.Debts.Commands.CreateDebtRecord;

using KasirIn.Application.Common.Interfaces;
using KasirIn.Domain.Entities;
using MediatR;

public class CreateDebtRecordCommandHandler : IRequestHandler<CreateDebtRecordCommand, Guid>
{
    private readonly IKasirInDbContext _context;

    public CreateDebtRecordCommandHandler(IKasirInDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateDebtRecordCommand request, CancellationToken cancellationToken)
    {
        var debtRecord = new DebtRecord
        {
            Id = Guid.NewGuid(),
            TenantId = request.TenantId,
            CustomerName = request.CustomerName,
            CustomerPhone = request.CustomerPhone,
            TotalDebt = request.TotalDebt,
            PaidDebt = request.PaidDebt,
            DueDate = request.DueDate,
            RemainingDebt = request.TotalDebt - request.PaidDebt,
            IsSettled = (request.TotalDebt - request.PaidDebt) <= 0
        };

        _context.DebtRecords.Add(debtRecord);
        await _context.SaveChangesAsync(cancellationToken);

        return debtRecord.Id;
    }
}
