using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Exceptions;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class SaleItem : BaseEntity
{
    public Guid SaleId { get; private set; }
    public string ProductExternalId { get; private set; } = string.Empty;
    public string ProductName { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal Discount { get; private set; }
    public decimal TotalAmount { get; private set; }
    public bool IsCancelled { get; private set; }

    protected SaleItem()
    {
    }

    public SaleItem(string productExternalId, string productName, int quantity, decimal unitPrice)
    {
        Update(productExternalId, productName, quantity, unitPrice);
    }

    public void AttachToSale(Guid saleId)
    {
        SaleId = saleId;
    }

    public void Update(string productExternalId, string productName, int quantity, decimal unitPrice)
    {
        if (string.IsNullOrWhiteSpace(productExternalId) || string.IsNullOrWhiteSpace(productName))
            throw new DomainException("Product information is required.");

        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero.");

        if (quantity > 20)
            throw new DomainException("It is not possible to sell more than 20 identical items.");

        if (unitPrice < 0)
            throw new DomainException("Unit price cannot be negative.");

        ProductExternalId = productExternalId.Trim();
        ProductName = productName.Trim();
        Quantity = quantity;
        UnitPrice = unitPrice;
        IsCancelled = false;

        var grossAmount = Quantity * UnitPrice;
        var discountRate = Quantity >= 10 ? 0.20m : Quantity >= 4 ? 0.10m : 0m;

        Discount = decimal.Round(grossAmount * discountRate, 2, MidpointRounding.AwayFromZero);
        TotalAmount = grossAmount - Discount;
    }

    public void Cancel()
    {
        if (IsCancelled)
            return;

        IsCancelled = true;
        TotalAmount = 0m;
    }
}
