using FinanceControl.Application.DTOs.Transaction;
using FinanceControl.Domain.Common;
using FinanceControl.Domain.Enums;

namespace FinanceControl.Application.Interfaces;

public interface ITransactionService
{
    Task<TransactionResponseDto> CreateAsync(Guid userId, CreateTransactionDto dto);
    Task<IEnumerable<TransactionResponseDto>> GetAllByUserIdAsync(
        Guid userId, TransactionType? type = null);
    Task<PagedResult<TransactionResponseDto>> GetPagedByUserIdAsync(
        Guid userId,
        PaginationParams pagination,
        TransactionFilter? filter = null);
    Task<TransactionResponseDto> GetByIdAsync(Guid userId, Guid transactionId);
    Task<TransactionResponseDto> UpdateAsync(
        Guid userId, Guid transactionId, UpdateTransactionDto dto);
    Task DeleteAsync(Guid userId, Guid transactionId);
}