using FinanceControl.Domain.Entities;

namespace FinanceControl.Domain.Interfaces.Repositories;

public interface IRefreshTokenRepository : IBaseRepository<RefreshToken>
{
    Task<RefreshToken?> GetByTokenAsync(string token);
}