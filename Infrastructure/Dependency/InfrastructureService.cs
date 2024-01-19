using Data.Persistence;
using Microsoft.EntityFrameworkCore;
using Data.Implementation.Repositories;
using Microsoft.Extensions.Configuration;
using Application.Interfaces.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Data.Dependency;

public static class InfrastructureService
{
    public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString,
                b => b.MigrationsAssembly("Infrastructure")));

        services.AddTransient<IGenericRepository, GenericRepository>();
        
        return services;
    }
}