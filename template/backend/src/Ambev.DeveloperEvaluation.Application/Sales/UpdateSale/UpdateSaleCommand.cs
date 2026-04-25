using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

public class UpdateSaleCommand : IRequest<SaleResult>
{
    public Guid Id { get; set; }
    public string SaleNumber { get; set; } = string.Empty;
    public DateTime SaleDate { get; set; }
    public string CustomerExternalId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string BranchExternalId { get; set; } = string.Empty;
    public string BranchName { get; set; } = string.Empty;
    public bool IsCancelled { get; set; }
    public List<SaleItemDto> Items { get; set; } = [];
}
