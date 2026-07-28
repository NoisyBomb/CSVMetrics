using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CSVMetrics.Application;


namespace CSVMetrics.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<AppDb>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IMeasurementRepository, MeasurementRepository>();
        services.AddScoped<IFileResultRepository, FileResultRepository>();
        services.AddScoped<ITransaction, Transaction>();
        return services;
    }
}