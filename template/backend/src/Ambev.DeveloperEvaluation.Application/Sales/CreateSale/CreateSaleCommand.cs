using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

public class CreateSaleCommand : IRequest<SaleResult>
{
    public string SaleNumber { get; set; } = string.Empty;
    public DateTime SaleDate { get; set; }
    public string CustomerExternalId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string BranchExternalId { get; set; } = string.Empty;
    public string BranchName { get; set; } = string.Empty;
    public List<SaleItemDto> Items { get; set; } = [];
}
