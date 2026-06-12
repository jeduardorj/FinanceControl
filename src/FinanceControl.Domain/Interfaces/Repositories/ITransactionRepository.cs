
using FinanceControl.Domain.Entities;

namespace FinanceControl.Domain.Interfaces.Repositories;

public interface ITransactionRepository : IBaseRepository<Transaction>
{
    Task<IEnumerable<Transaction>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<Transaction>> GetByUserIdAndPeriodAsync(
        Guid userId,
        DateTime startDate,
        DateTime endDate);
}