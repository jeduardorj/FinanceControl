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
        Guid userId,
        PaginationParams pagination,
        TransactionFilter? filter = null)
    {
        // IQueryable — query ainda não executada
        var query = _dbSet
            .Where(t => t.UserId == userId)
            .Include(t => t.Category)
            .AsQueryable();

        // Aplica filtros condicionalmente — tudo vira SQL
        if (filter != null)
        {
            if (filter.Type.HasValue)
                query = query.Where(t => t.Type == filter.Type.Value);

            if (filter.StartDate.HasValue)
                query = query.Where(t => t.Date >= filter.StartDate.Value);

            if (filter.EndDate.HasValue)
                query = query.Where(t => t.Date <= filter.EndDate.Value);

            if (filter.CategoryId.HasValue)
                query = query.Where(t => t.CategoryId == filter.CategoryId.Value);

            if (filter.MinAmount.HasValue)
                query = query.Where(t => t.Amount >= filter.MinAmount.Value);

            if (filter.MaxAmount.HasValue)
                query = query.Where(t => t.Amount <= filter.MaxAmount.Value);

            if (!string.IsNullOrEmpty(filter.Description))
                query = query.Where(t => t.Description
                    .Contains(filter.Description));
        }

        // Ordena
        query = query.OrderByDescending(t => t.Date);

        // Conta ANTES de paginar — total real com filtros aplicados
        var totalCount = await query.CountAsync();

        // Pagina DEPOIS de filtrar
        var items = await query
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync();

        return PagedResult<Transaction>.Create(
            items, totalCount, pagination.PageNumber, pagination.PageSize);
    }
}