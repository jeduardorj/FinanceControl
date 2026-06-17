using FinanceControl.Domain.Interfaces.Repositories;

namespace FinanceControl.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    ICategoryRepository Categories { get; }
    ITransactionRepository Transactions { get; }
    IRefreshTokenRepository RefreshTokens { get; }
    Task<int> CommitAsync();
}