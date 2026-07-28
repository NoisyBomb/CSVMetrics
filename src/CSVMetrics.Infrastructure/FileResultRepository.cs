using CSVMetrics.Application;
using CSVMetrics.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CSVMetrics.Infrastructure;

public class FileResultRepository : IFileResultRepository
{
    private readonly AppDb _context;
    public FileResultRepository(AppDb context)
    {
        _context = context;
    }
    public async Task<FileResult?> GetByFileNameAsync(string fileName)
    {
        return await _context.Results.FirstOrDefaultAsync(r => r.FileName == fileName);
    }
    public async Task AddAsync(FileResult result)
    {
        await _context.Results.AddAsync(result);
    }
    public async Task DeleteAsync(FileResult result)
    {
        _context.Results.Remove(result);
        await Task.CompletedTask;
    }
}