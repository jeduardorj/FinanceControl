using FinanceControl.Domain.Entities;
using FinanceControl.Domain.Interfaces.Repositories;
using FinanceControl.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinanceControl.Infrastructure.Repositories;

public class RefreshTokenRepository : BaseRepository<RefreshToken>, IRefreshTokenRepository
{
    public RefreshTokenRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        return await _dbSet
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Token == token);
    }
}