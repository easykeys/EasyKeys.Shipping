using EasyKeys.Shipping.Stamps.Abstractions.Options;
using EasyKeys.Shipping.Stamps.Abstractions.Services;
using EasyKeys.Shipping.Stamps.Abstractions.Services.Impl;

using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

public static class StampsServiceCollectionExtensions
{
    public static IServiceCollection AddStampsClient(
        this IServiceCollection services,
        string sectionName = nameof(StampsOptions),
        Action<StampsOptions, IServiceProvider>? configure = null)
    {
        services.AddChangeTokenOptions<StampsOptions>(sectionName, null, (options, config) => configure?.Invoke(options, config));

        services.TryAddScoped<IStampsClientService, StampsClientService>();

        return services;
    }

    public static IServiceCollection AddStampsRate(
        this IServiceCollection services,
        string sectionName = nameof(StampsOptions),
        Action<StampsOptions, IServiceProvider>? configure = null)
    {
        services.AddChangeTokenOptions<StampsOptions>(sectionName, null, (options, config) => configure?.Invoke(options, config));

        services.TryAddScoped<IRatesService, RatesService>();

        return services;
    }
}
