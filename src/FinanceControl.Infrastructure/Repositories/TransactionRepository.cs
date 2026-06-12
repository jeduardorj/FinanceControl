using FinanceControl.Domain.Entities;
using FinanceControl.Domain.Interfaces.Repositories;
using FinanceControl.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinanceControl.Infrastructure.Repositories;

public class TransactionRepository : BaseRepository<Transaction>, ITransactionRepository
{
    public TransactionRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Transaction>> GetByUserIdAsync(Guid userId)
    {
        return await _dbSet
            .Where(t => t.UserId == userId)
            .Include(t => t.Category)
            .OrderByDescending(t => t.Date)
            .ToListAsync();
    }

    public async Task<IEnumerable<Transaction>> GetByUserIdAndPeriodAsync(
        Guid userId,
        DateTime startDate,
        DateTime endDate)
    {
        return await _dbSet
            .Where(t => t.UserId == userId &&
                        t.Date >= startDate &&
                        t.Date <= endDate)
            .Include(t => t.Category)
            .OrderByDescending(t => t.Date)
            .ToListAsync();
    }
}