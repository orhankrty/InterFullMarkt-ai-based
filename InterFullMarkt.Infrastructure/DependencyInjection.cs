namespace InterFullMarkt.Infrastructure;

using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using InterFullMarkt.Infrastructure.Data;
using InterFullMarkt.Infrastructure.Data.Interceptors;
using InterFullMarkt.Application.Abstractions;

/// <summary>
/// Infrastructure katmanının Dependency Injection uzantılarını içerir.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Infrastructure katmanını Service Container'a kaydeder.
    /// </summary>
    /// <param name="services">IServiceCollection</param>
    /// <param name="databasePath">SQLite veritabanı dosyasının yolu (isteğe bağlı)</param>
    /// <returns>Düzenlenmiş IServiceCollection</returns>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string databasePath = "./Data/FullMarkt.db")
    {
        // DbContext Yapılandırması
        var connectionString = $"Data Source={databasePath}";

        services.AddDbContext<InterFullMarktDbContext>(options =>
        {
            options.UseSqlite(connectionString)
                .AddInterceptors(new AuditInterceptor())
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development");
        });

        // IDbContext abstraksionu için registration
        services.AddScoped<IDbContext>(provider => provider.GetRequiredService<InterFullMarktDbContext>());

        return services;
    }

    /// <summary>
    /// Veritabanını otomatik olarak migrate eder ve oluşturur.
    /// </summary>
    /// <param name="app">IApplicationBuilder</param>
    /// <returns>Düzenlenmiş IApplicationBuilder</returns>
    public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<InterFullMarktDbContext>();

        // Veritabanını oluştur ve migrate et
        dbContext.Database.Migrate();

        return app;
    }
}
