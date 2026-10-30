using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SGCAE.Application.Abstractions;
using SGCAE.Application.Access;
using SGCAE.Application.AccessPoints;
using SGCAE.Application.Credentials;
using SGCAE.Application.Employees;
using SGCAE.Infrastructure.Persistence;
using SGCAE.Infrastructure.Queries;
using SGCAE.Infrastructure.Repositories;

namespace SGCAE.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<SgcaeDbContext>(o => o.UseSqlServer(connectionString));
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<SgcaeDbContext>());

        // Escritura
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<ICredentialRepository, CredentialRepository>();
        services.AddScoped<IAccessPointRepository, AccessPointRepository>();
        services.AddScoped<IAccessRecordRepository, AccessRecordRepository>();

        // Lectura
        services.AddScoped<IEmployeeQueries, EmployeeQueries>();
        services.AddScoped<ICredentialQueries, CredentialQueries>();
        services.AddScoped<IAccessPointQueries, AccessPointQueries>();
        services.AddScoped<IAccessRecordQueries, AccessRecordQueries>();

        return services;
    }
}
