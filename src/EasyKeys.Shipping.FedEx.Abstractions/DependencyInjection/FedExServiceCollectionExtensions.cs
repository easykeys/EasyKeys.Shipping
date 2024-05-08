using EasyKeys.Shipping.FedEx.Abstractions.Api.V1.Auth;
using EasyKeys.Shipping.FedEx.Abstractions.Api.V1.Auth.Impl;
using EasyKeys.Shipping.FedEx.Abstractions.Middleware;
using EasyKeys.Shipping.FedEx.Abstractions.Options;
using EasyKeys.Shipping.FedEx.Abstractions.Services;
using EasyKeys.Shipping.FedEx.Abstractions.Services.Impl;

using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

public static class FedExServiceCollectionExtensions
{
    /// <summary>
    /// Adds FedEx Client Service with configuration options <see cref="FedExOptions"/>.
    /// </summary>
    /// <param name="services">The DI services.</param>
    /// <param name="sectionName">The section name for the options.</param>
    /// <param name="configure">The configuration of options.</param>
    /// <returns></returns>
    public static IServiceCollection AddFedExClient(
        this IServiceCollection services,
        string sectionName = nameof(FedExOptions),
        Action<FedExOptions, IServiceProvider>? configure = null)
    {
        services.AddChangeTokenOptions<FedExOptions>(sectionName, null, (options, config) => configure?.Invoke(options, config));

        services.TryAddScoped<IFedExClientService, FedExClientService>();

        return services;
    }

    /// <summary>
    /// Adds <see cref="IFedExAuthClient"/> and <see cref="AuthRequestMiddleware"/> with configuration options <see cref="FedExApiOptions"/>.
    /// </summary>
    /// <param name="services">The DI services.</param>
    /// <param name="sectionName">The section name for the options.</param>
    /// <param name="configure">The configuration of options.</param>
    /// <returns></returns>
    public static IServiceCollection AddFedExAuthApiClient(
        this IServiceCollection services,
        string sectionName = nameof(FedExApiOptions),
        Action<FedExApiOptions, IServiceProvider>? configure = null)
    {
        services.AddChangeTokenOptions<FedExApiOptions>(sectionName, null, (options, config) => configure?.Invoke(options, config));
        services.AddHttpClient(name: nameof(IFedExAuthClient));
        services.AddTransient<AuthRequestMiddleware>();
        services.AddSingleton<IFedExAuthClient, FedExAuthClient>();

        return services;
    }
}
