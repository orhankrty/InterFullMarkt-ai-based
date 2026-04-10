namespace InterFullMarkt.Application;

using FluentValidation;
using MediatR;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using InterFullMarkt.Application.Common.Behaviors;
using InterFullMarkt.Application.Mappings;
using InterFullMarkt.Application.Features.Players.Commands.CreatePlayer;

/// <summary>
/// Application katmanının Dependency Injection ayarları.
/// MediatR, AutoMapper, FluentValidation ve Pipeline Behaviors'ı register eder.
/// </summary>
public static class ServiceRegistration
{
    /// <summary>
    /// Application katmanını Service Container'a register eder.
    /// </summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // AutoMapper yapılandırması
        services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile<PlayerProfile>();
            // İleride ek profiller buraya eklenecek
        });

        // MediatR Konfigürasyonu
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(ServiceRegistration).Assembly);
            
            // Pipeline Behaviors kaydı (Validasyon, Logging, vb.)
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        // FluentValidation Konfigürasyonu
        services.AddValidatorsFromAssemblyContaining<ServiceRegistration>(includeInternalTypes: true);

        return services;
    }
}
