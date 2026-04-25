namespace Ambev.DeveloperEvaluation.Application.Sales;

public class SaleItemDto
{
    public Guid? Id { get; set; }
    public string ProductExternalId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public bool IsCancelled { get; set; }
}
