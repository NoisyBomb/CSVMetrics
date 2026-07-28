namespace CSVMetrics.Application;

public interface ITransaction
{
    Task<IDisposable> BeginTransactionAsync();
    Task SaveChangesAsync();
    Task CommitAsync();
}