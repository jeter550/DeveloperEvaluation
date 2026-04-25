using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.ListSales;

public record ListSalesQuery() : IRequest<IReadOnlyList<SaleResult>>;
