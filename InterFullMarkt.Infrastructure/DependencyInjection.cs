namespace InterFullMarkt.Infrastructure;

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using InterFullMarkt.Infrastructure.Data;
using InterFullMarkt.Application.Abstractions;
using InterFullMarkt.Infrastructure.Identity;

/// <summary>
/// Infrastructure katmanının Dependency Injection uzantılarını içerir.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Infrastructure katmanını Service Container'a kaydeder.
    /// </summary>
    /// <param name="services">IServiceCollection</param>
    /// <param name="connectionString">Veritabanı bağlantı dizesi</param>
    /// <returns>Düzenlenmiş IServiceCollection</returns>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        // DbContext Yapılandırması
        services.AddDbContext<InterFullMarktDbContext>(options =>
        {
            options.UseSqlite(connectionString)
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development");
        });

        // IDbContext abstraksionu için registration
        services.AddScoped<IDbContext>(provider => provider.GetRequiredService<InterFullMarktDbContext>());

        // Auth Service Kaydı
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}
