namespace KasirIn.Application.Products.Commands.CreateProduct;

using FluentValidation;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty().WithMessage("TenantId wajib diisi.");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("CategoryId wajib diisi.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nama produk wajib diisi.")
            .MaximumLength(150).WithMessage("Nama produk maksimal 150 karakter.");

        RuleFor(x => x.SKU)
            .NotEmpty().WithMessage("SKU wajib diisi.")
            .MaximumLength(50).WithMessage("SKU maksimal 50 karakter.");

        RuleFor(x => x.CostPrice)
            .GreaterThanOrEqualTo(0).WithMessage("Harga modal tidak boleh negatif.");

        RuleFor(x => x.SellingPrice)
            .GreaterThanOrEqualTo(0).WithMessage("Harga jual tidak boleh negatif.");

        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("Jumlah stok tidak boleh negatif.");

        RuleFor(x => x.MinStockThreshold)
            .GreaterThanOrEqualTo(0).WithMessage("Batas minimal stok tidak boleh negatif.");
    }
}
