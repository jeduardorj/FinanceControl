using FinanceControl.Domain.Common;
using FinanceControl.Domain.Entities;
using FinanceControl.Domain.Interfaces.Repositories;
using FinanceControl.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinanceControl.Infrastructure.Repositories;

public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
{
    public CategoryRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Category>> GetByUserIdAsync(Guid userId)
    {
        return await _dbSet
            .Where(c => c.UserId == userId)
            .ToListAsync();
    }

    public async Task<PagedResult<Category>> GetPagedByUserIdAsync(
        Guid userId, PaginationParams pagination)
    {
        var query = _dbSet
            .Where(c => c.UserId == userId)
            .OrderBy(c => c.Name);

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync();

        return PagedResult<Category>.Create(
            items, totalCount, pagination.PageNumber, pagination.PageSize);
    }
}