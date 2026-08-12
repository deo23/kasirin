namespace KasirIn.Application.Debts.Commands.CreateDebtRecord;

using FluentValidation;

public class CreateDebtRecordCommandValidator : AbstractValidator<CreateDebtRecordCommand>
{
    public CreateDebtRecordCommandValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty().WithMessage("TenantId wajib diisi.");

        RuleFor(x => x.CustomerName)
            .NotEmpty().WithMessage("Nama pelanggan wajib diisi.")
            .MaximumLength(150).WithMessage("Nama pelanggan maksimal 150 karakter.");

        RuleFor(x => x.TotalDebt)
            .GreaterThan(0).WithMessage("Total hutang harus lebih dari 0.");

        RuleFor(x => x.PaidDebt)
            .GreaterThanOrEqualTo(0).WithMessage("Hutang dibayar tidak boleh negatif.")
            .LessThanOrEqualTo(x => x.TotalDebt).WithMessage("Hutang dibayar tidak boleh melebihi total hutang.");
    }
}
