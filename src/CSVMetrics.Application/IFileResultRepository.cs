using CSVMetrics.Domain.Entities;
namespace CSVMetrics.Application;

public interface IFileResultRepository
{
    Task<FileResult?> GetByFileNameAsync(string fileName);
    Task AddAsync(FileResult result);
    Task DeleteAsync(FileResult result);
}