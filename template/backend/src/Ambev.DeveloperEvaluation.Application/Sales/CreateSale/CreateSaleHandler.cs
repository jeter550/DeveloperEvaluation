using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

public class CreateSaleHandler : IRequestHandler<CreateSaleCommand, SaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateSaleHandler> _logger;

    public CreateSaleHandler(ISaleRepository saleRepository, IMapper mapper, ILogger<CreateSaleHandler> logger)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<SaleResult> Handle(CreateSaleCommand command, CancellationToken cancellationToken)
    {
        var existingSale = await _saleRepository.GetBySaleNumberAsync(command.SaleNumber, cancellationToken);
        if (existingSale is not null)
            throw new InvalidOperationException($"Sale number {command.SaleNumber} already exists.");

        var sale = new Sale(
            command.SaleNumber,
            command.SaleDate,
            command.CustomerExternalId,
            command.CustomerName,
            command.BranchExternalId,
            command.BranchName,
            command.Items.Select(MapItem));

        var createdSale = await _saleRepository.CreateAsync(sale, cancellationToken);
        _logger.LogInformation("Event {EventName}: {@EventPayload}", nameof(SaleCreatedEvent), new SaleCreatedEvent(createdSale.Id, createdSale.SaleNumber));

        return _mapper.Map<SaleResult>(createdSale);
    }

    private static SaleItem MapItem(SaleItemDto item) =>
        new(item.ProductExternalId, item.ProductName, item.Quantity, item.UnitPrice);
}
