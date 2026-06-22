using FinanceControl.Domain.Common;
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
        Guid userId, DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Where(t => t.UserId == userId &&
                        t.Date >= startDate &&
                        t.Date <= endDate)
            .Include(t => t.Category)
            .OrderByDescending(t => t.Date)
            .ToListAsync();
    }

    public async Task<IEnumerable<Transaction>> GetByUserIdAndMonthAsync(
        Guid userId, int year, int month)
    {
        return await _dbSet
            .Where(t => t.UserId == userId &&
                        t.Date.Year == year &&
                        t.Date.Month == month)
            .Include(t => t.Category)
            .ToListAsync();
    }

    public async Task<PagedResult<Transaction>> GetPagedByUserIdAsync(
        Guid userId, PaginationParams pagination)
    {
        var query = _dbSet
            .Where(t => t.UserId == userId)
            .Include(t => t.Category)
            .OrderByDescending(t => t.Date);

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync();

        return PagedResult<Transaction>.Create(
            items, totalCount, pagination.PageNumber, pagination.PageSize);
    }
}