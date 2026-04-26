using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Exceptions;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class Sale : BaseEntity
{
    private readonly List<SaleItem> _items = [];

    public string SaleNumber { get; private set; } = string.Empty;
    public DateTime SaleDate { get; private set; }
    public string CustomerExternalId { get; private set; } = string.Empty;
    public string CustomerName { get; private set; } = string.Empty;
    public string BranchExternalId { get; private set; } = string.Empty;
    public string BranchName { get; private set; } = string.Empty;
    public bool IsCancelled { get; private set; }
    public decimal TotalAmount { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public IReadOnlyCollection<SaleItem> Items => _items;

    protected Sale()
    {
    }

    public Sale(
        string saleNumber,
        DateTime saleDate,
        string customerExternalId,
        string customerName,
        string branchExternalId,
        string branchName,
        IEnumerable<SaleItem> items)
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        UpdateDetails(saleNumber, saleDate, customerExternalId, customerName, branchExternalId, branchName, items);
    }

    public void UpdateDetails(
        string saleNumber,
        DateTime saleDate,
        string customerExternalId,
        string customerName,
        string branchExternalId,
        string branchName,
        IEnumerable<SaleItem> items)
    {
        if (string.IsNullOrWhiteSpace(saleNumber))
            throw new DomainException("Sale number is required.");

        if (string.IsNullOrWhiteSpace(customerExternalId) || string.IsNullOrWhiteSpace(customerName))
            throw new DomainException("Customer information is required.");

        if (string.IsNullOrWhiteSpace(branchExternalId) || string.IsNullOrWhiteSpace(branchName))
            throw new DomainException("Branch information is required.");

        var normalizedItems = items?.ToList() ?? [];
        if (normalizedItems.Count == 0)
            throw new DomainException("A sale must contain at least one item.");

        SaleNumber = saleNumber.Trim();
        SaleDate = saleDate;
        CustomerExternalId = customerExternalId.Trim();
        CustomerName = customerName.Trim();
        BranchExternalId = branchExternalId.Trim();
        BranchName = branchName.Trim();

        _items.Clear();
        foreach (var item in normalizedItems)
        {
            item.AttachToSale(Id);
            _items.Add(item);
        }

        IsCancelled = _items.All(item => item.IsCancelled);
        Touch();
        RecalculateTotal();
    }

    public void Cancel()
    {
        if (IsCancelled)
            return;

        foreach (var item in _items)
        {
            item.Cancel();
        }

        IsCancelled = true;
        Touch();
        RecalculateTotal();
    }

    public void CancelItem(Guid itemId)
    {
        var item = _items.FirstOrDefault(x => x.Id == itemId)
            ?? throw new DomainException("Sale item was not found.");

        item.Cancel();
        IsCancelled = _items.All(x => x.IsCancelled);
        Touch();
        RecalculateTotal();
    }

    public void RecalculateTotal()
    {
        TotalAmount = _items.Where(x => !x.IsCancelled).Sum(x => x.TotalAmount);
    }

    private void Touch()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}
