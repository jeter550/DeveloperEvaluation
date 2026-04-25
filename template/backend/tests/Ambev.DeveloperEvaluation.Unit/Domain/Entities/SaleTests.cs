using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

public class SaleTests
{
    [Fact(DisplayName = "Given quantity below four when creating item then discount should be zero")]
    public void SaleItem_WithQuantityBelowFour_ShouldNotApplyDiscount()
    {
        var item = new SaleItem("product-1", "Product 1", 3, 10m);

        item.Discount.Should().Be(0m);
        item.TotalAmount.Should().Be(30m);
    }

    [Fact(DisplayName = "Given quantity between four and nine when creating item then should apply ten percent discount")]
    public void SaleItem_WithQuantityBetweenFourAndNine_ShouldApplyTenPercentDiscount()
    {
        var item = new SaleItem("product-1", "Product 1", 4, 10m);

        item.Discount.Should().Be(4m);
        item.TotalAmount.Should().Be(36m);
    }

    [Fact(DisplayName = "Given quantity between ten and twenty when creating item then should apply twenty percent discount")]
    public void SaleItem_WithQuantityBetweenTenAndTwenty_ShouldApplyTwentyPercentDiscount()
    {
        var item = new SaleItem("product-1", "Product 1", 10, 10m);

        item.Discount.Should().Be(20m);
        item.TotalAmount.Should().Be(80m);
    }

    [Fact(DisplayName = "Given quantity above twenty when creating item then should throw domain exception")]
    public void SaleItem_WithQuantityAboveTwenty_ShouldThrowDomainException()
    {
        var action = () => new SaleItem("product-1", "Product 1", 21, 10m);

        action.Should().Throw<DomainException>()
            .WithMessage("It is not possible to sell more than 20 identical items.");
    }

    [Fact(DisplayName = "Given sale cancellation when cancelling sale then all items and total should be cancelled")]
    public void Sale_WhenCancelled_ShouldCancelItemsAndZeroTotal()
    {
        var sale = new Sale(
            "SALE-001",
            DateTime.UtcNow,
            "customer-1",
            "Customer 1",
            "branch-1",
            "Branch 1",
            [
                new SaleItem("product-1", "Product 1", 4, 10m),
                new SaleItem("product-2", "Product 2", 1, 5m)
            ]);

        sale.Cancel();

        sale.IsCancelled.Should().BeTrue();
        sale.TotalAmount.Should().Be(0m);
        sale.Items.Should().OnlyContain(item => item.IsCancelled);
    }
}
