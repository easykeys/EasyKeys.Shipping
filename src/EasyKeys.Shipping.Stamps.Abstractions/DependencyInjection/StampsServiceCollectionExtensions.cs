
using EasyKeys.Shipping.Stamps.Abstractions.Options;
using EasyKeys.Shipping.Stamps.Abstractions.Services;
using EasyKeys.Shipping.Stamps.Abstractions.Services.Impl;

using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

public static class StampsServiceCollectionExtensions
{
    /// <summary>
    /// Adds Stamps.com authentication service.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="sectionName"></param>
    /// <param name="configure"></param>
    /// <returns></returns>
    public static IServiceCollection AddStampsClient(
        this IServiceCollection services,
        string sectionName = nameof(StampsOptions),
        Action<StampsOptions, IServiceProvider>? configure = null)
    {
        services.AddChangeTokenOptions<StampsOptions>(sectionName, null, (options, config) => configure?.Invoke(options, config));

        services.TryAddSingleton<IPolicyService, PolicyService>();

        services.TryAddSingleton<IStampsClientService, StampsClientService>();

        return services;
    }
}
