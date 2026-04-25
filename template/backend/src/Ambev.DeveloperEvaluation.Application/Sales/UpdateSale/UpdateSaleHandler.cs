using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

public class UpdateSaleHandler : IRequestHandler<UpdateSaleCommand, SaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateSaleHandler> _logger;

    public UpdateSaleHandler(ISaleRepository saleRepository, IMapper mapper, ILogger<UpdateSaleHandler> logger)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<SaleResult> Handle(UpdateSaleCommand command, CancellationToken cancellationToken)
    {
        var sale = await _saleRepository.GetByIdAsync(command.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Sale not found.");
        var duplicatedSale = await _saleRepository.GetBySaleNumberAsync(command.SaleNumber, cancellationToken);
        if (duplicatedSale is not null && duplicatedSale.Id != command.Id)
            throw new InvalidOperationException($"Sale number {command.SaleNumber} already exists.");

        var wasCancelled = sale.IsCancelled;

        var replacementItems = command.Items.Select(item =>
        {
            var saleItem = new SaleItem(item.ProductExternalId, item.ProductName, item.Quantity, item.UnitPrice);
            if (item.IsCancelled)
                saleItem.Cancel();

            return saleItem;
        }).ToList();

        sale.UpdateDetails(
            command.SaleNumber,
            command.SaleDate,
            command.CustomerExternalId,
            command.CustomerName,
            command.BranchExternalId,
            command.BranchName,
            replacementItems);

        foreach (var cancelledItem in replacementItems.Where(x => x.IsCancelled))
        {
            _logger.LogInformation(
                "Event {EventName}: {@EventPayload}",
                nameof(ItemCancelledEvent),
                new ItemCancelledEvent(sale.Id, cancelledItem.ProductExternalId, sale.SaleNumber));
        }

        if (command.IsCancelled)
        {
            sale.Cancel();
        }

        if (sale.IsCancelled && !wasCancelled)
        {
            _logger.LogInformation("Event {EventName}: {@EventPayload}", nameof(SaleCancelledEvent), new SaleCancelledEvent(sale.Id, sale.SaleNumber));
        }
        else
        {
            _logger.LogInformation("Event {EventName}: {@EventPayload}", nameof(SaleModifiedEvent), new SaleModifiedEvent(sale.Id, sale.SaleNumber));
        }

        var updatedSale = await _saleRepository.UpdateAsync(sale, cancellationToken);
        return _mapper.Map<SaleResult>(updatedSale);
    }
}
