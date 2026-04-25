using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales;

public class SaleItemDtoValidator : AbstractValidator<SaleItemDto>
{
    public SaleItemDtoValidator()
    {
        RuleFor(x => x.ProductExternalId).NotEmpty();
        RuleFor(x => x.ProductName).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0).LessThanOrEqualTo(20);
        RuleFor(x => x.UnitPrice).GreaterThanOrEqualTo(0);
    }
}
