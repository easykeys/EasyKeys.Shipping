using EasyKeys.Shipping.Stamps.Abstractions.Options;
using EasyKeys.Shipping.Stamps.Abstractions.Services;
using EasyKeys.Shipping.Stamps.Abstractions.Services.Impl;

using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

public static class StampsServiceCollectionExtensions
{
    /// <summary>
    /// Adds Stamps.com authentication service and default soap client with polly policy.
    /// </summary>
    /// <param name="services">The DI services.</param>
    /// <param name="sectionName">The name of the configuration section. The default <see cref="StampsOptions"/>.</param>
    /// <param name="configure">The configure options.</param>
    /// <returns></returns>
    public static IServiceCollection AddStampsClient(
        this IServiceCollection services,
        string sectionName = nameof(StampsOptions),
        Action<StampsOptions, IServiceProvider>? configure = null)
    {
        services.AddChangeTokenOptions<StampsOptions>(sectionName, null, (options, config) => configure?.Invoke(options, config));

        services.TryAddSingleton<IStampsClientService, StampsClientService>();
        services.TryAddSingleton<IStampsClientAuthenticator, StampsClientAuthenticator>();

        return services;
    }
}
