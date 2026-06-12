using FinanceControl.Domain.Interfaces;
using FinanceControl.Domain.Interfaces.Repositories;
using FinanceControl.Infrastructure.Data;

namespace FinanceControl.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public IUserRepository Users { get; }
    public ICategoryRepository Categories { get; }
    public ITransactionRepository Transactions { get; }

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        Users = new UserRepository(context);
        Categories = new CategoryRepository(context);
        Transactions = new TransactionRepository(context);
    }

    public async Task<int> CommitAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}