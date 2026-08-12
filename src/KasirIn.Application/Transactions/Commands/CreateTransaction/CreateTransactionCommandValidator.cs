namespace KasirIn.Application.Transactions.Commands.CreateTransaction;

using FluentValidation;

public class CreateTransactionCommandValidator : AbstractValidator<CreateTransactionCommand>
{
    public CreateTransactionCommandValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty().WithMessage("TenantId wajib diisi.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId wajib diisi.");

        RuleFor(x => x.PaymentMethod)
            .NotEmpty().WithMessage("Metode pembayaran wajib diisi.");

        RuleFor(x => x.PaidAmount)
            .GreaterThanOrEqualTo(0).WithMessage("Jumlah bayar tidak boleh negatif.");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("Items transaksi tidak boleh kosong.");

        RuleForEach(x => x.Items).ChildRules(items =>
        {
            items.RuleFor(i => i.ProductId)
                .NotEmpty().WithMessage("ProductId wajib diisi.");

            items.RuleFor(i => i.Quantity)
                .GreaterThan(0).WithMessage("Jumlah item harus lebih dari 0.");
        });
    }
}
