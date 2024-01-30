using Data.Persistence;
using Microsoft.EntityFrameworkCore;
using Data.Implementation.Repositories;
using Microsoft.Extensions.Configuration;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Data.Implementation.Services;
using Data.Persistence.Seed;
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

        services.AddTransient<IConfigurationService, ConfigurationService>();
        services.AddTransient<IContentService, ContentService>();
        services.AddTransient<IDbInitializerService, DbInitializerService>();
        services.AddTransient<IEBookService, EBookService>();
        services.AddTransient<IEnrollmentService, EnrollmentService>();
        services.AddTransient<INotificationService, NotificationService>();
        services.AddTransient<IQuestionService, QuestionService>();
        services.AddTransient<IStudentService, StudentService>();
        services.AddTransient<IUserService, UserService>();
        services.AddTransient<INewsAndAlertService, NewsAndAlertService>();

        return services;
    }
}