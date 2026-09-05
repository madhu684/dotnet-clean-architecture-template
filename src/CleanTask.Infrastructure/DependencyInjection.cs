using CleanTask.Application.Common.Interfaces;
using CleanTask.Domain.Interfaces;
using CleanTask.Infrastructure.Persistence;
using CleanTask.Infrastructure.Repositories;
using CleanTask.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CleanTask.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // --- Database: supports both SQL Server and PostgreSQL ---
        // Set "DatabaseProvider": "SqlServer" or "PostgreSQL" in appsettings.json
        var provider = configuration.GetValue<string>("DatabaseProvider") ?? "SqlServer";

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            if (provider == "PostgreSQL")
            {
                options.UseNpgsql(
                    configuration.GetConnectionString("PostgreSQLConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
            }
            else
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
            }
        });

        // Unit of Work & Repositories
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ITaskRepository, TaskRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        // Services
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IPasswordService, PasswordService>();

        return services;
    }
}
