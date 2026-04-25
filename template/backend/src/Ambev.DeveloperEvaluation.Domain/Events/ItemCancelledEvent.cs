namespace Ambev.DeveloperEvaluation.Domain.Events;

public record ItemCancelledEvent(Guid SaleId, string ProductExternalId, string SaleNumber);
