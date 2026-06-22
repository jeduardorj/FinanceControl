using FinanceControl.Domain.Enums;

namespace FinanceControl.Domain.Common;

public class TransactionFilter
{
    public TransactionType? Type { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public Guid? CategoryId { get; set; }
    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
    public string? Description { get; set; }
}