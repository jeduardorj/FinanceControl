using FinanceControl.Domain.Enums;

namespace FinanceControl.Application.DTOs.Transaction;

public class CreateTransactionDto
{
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public TransactionType Type { get; set; }
    public Guid CategoryId { get; set; }
}