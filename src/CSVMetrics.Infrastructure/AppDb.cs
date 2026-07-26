using Microsoft.EntityFrameworkCore;
using CSVMetrics.Domain.Entities;

namespace CSVMetrics.Infrastructure;

public class AppDb : DbContext
{
    public AppDb(DbContextOptions<AppDb> options) : base(options){}
    
    public DbSet<MeasurementValue> Values => Set<MeasurementValue>();
    public DbSet<FileResult> Results => Set<FileResult>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDb).Assembly);
    }
}