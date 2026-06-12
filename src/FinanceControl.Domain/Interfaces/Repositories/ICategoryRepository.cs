using FinanceControl.Domain.Entities;

namespace FinanceControl.Domain.Interfaces.Repositories;

public interface ICategoryRepository : IBaseRepository<Category>
{
    Task<IEnumerable<Category>> GetByUserIdAsync(Guid userId);
}