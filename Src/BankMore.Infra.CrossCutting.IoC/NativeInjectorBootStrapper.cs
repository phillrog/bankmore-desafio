using BankMore.Domain.Common.Interfaces;
using BankMore.Domain.Core.Bus;
using BankMore.Domain.Core.Notifications;
using BankMore.Domain.Common.Providers.Http;
using BankMore.Infra.CrossCutting.Bus;
using BankMore.Infra.CrossCutting.Identity.Authorization;
using BankMore.Infra.CrossCutting.Identity.Models;
using BankMore.Infra.CrossCutting.Identity.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using BankMore.Infra.CrossCutting.Identity.Filters;

namespace BankMore.Infra.CrossCutting.IoC;

public class NativeInjectorBootStrapper
{
    public static void RegisterServices(IServiceCollection services)
    {

        // ASP.NET HttpContext dependency
        services.AddHttpContextAccessor();

        // services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        // Domain Bus (Mediator)
        services.AddScoped<IMediatorHandler, InMemoryBus>();

        // ASP.NET Authorization Polices
        services.AddSingleton<IAuthorizationHandler, ClaimsRequirementHandler>();

        /// ASP.NET Authorization Role - Handler para identificar quem está reuquisitando os recursos (Master, Admin)
        services.AddSingleton<IAuthorizationHandler, MustBeOwnerOrMasterHandler>();

        // Domain - Events
        services.AddScoped<INotificationHandler<DomainNotification>, DomainNotificationHandler>();

        // Domain - Providers, 3rd parties
        services.AddScoped<IHttpProvider, HttpProvider>();

        // Infra - Identity
        services.AddScoped<IUser, AspNetUser>();
        services.AddSingleton<IJwtFactory, JwtFactory>();

    }
}
