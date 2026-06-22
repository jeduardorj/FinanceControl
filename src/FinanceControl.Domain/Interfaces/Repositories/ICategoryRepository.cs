using FinanceControl.Domain.Common;
using FinanceControl.Domain.Entities;

namespace FinanceControl.Domain.Interfaces.Repositories;

public interface ICategoryRepository : IBaseRepository<Category>
{
    Task<IEnumerable<Category>> GetByUserIdAsync(Guid userId);
    Task<PagedResult<Category>> GetPagedByUserIdAsync(
        Guid userId, PaginationParams pagination);
}