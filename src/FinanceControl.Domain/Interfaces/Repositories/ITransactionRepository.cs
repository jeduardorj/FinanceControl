using FinanceControl.Domain.Common;
using FinanceControl.Domain.Entities;

namespace FinanceControl.Domain.Interfaces.Repositories;

public interface ITransactionRepository : IBaseRepository<Transaction>
{
    Task<IEnumerable<Transaction>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<Transaction>> GetByUserIdAndPeriodAsync(
        Guid userId, DateTime startDate, DateTime endDate);
    Task<IEnumerable<Transaction>> GetByUserIdAndMonthAsync(
        Guid userId, int year, int month);
    Task<PagedResult<Transaction>> GetPagedByUserIdAsync(
        Guid userId,
        PaginationParams pagination,
        TransactionFilter? filter = null);
}