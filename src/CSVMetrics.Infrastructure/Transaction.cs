using CSVMetrics.Application;
using Microsoft.EntityFrameworkCore.Storage;

namespace CSVMetrics.Infrastructure;

public class Transaction : ITransaction
{
    private readonly AppDb _context;
    private IDbContextTransaction? _transaction;
    public Transaction(AppDb context)
    {
        _context = context;
    }
    public async Task<IDisposable> BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
        return _transaction;
    }
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
    public async Task CommitAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
        }
    }
}