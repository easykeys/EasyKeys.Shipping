using EasyKeys.Shipping.Stamps.Abstractions.Options;
using EasyKeys.Shipping.Stamps.Rates;

using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

public static class StampsRatesServiceCollectionExtensions
{
    public static IServiceCollection AddStampsRateProvider(
        this IServiceCollection services,
        string sectionName = nameof(StampsOptions),
        Action<StampsOptions, IServiceProvider>? configure = null)
    {
        services.AddChangeTokenOptions<StampsOptions>(sectionName, null, (options, config) => configure?.Invoke(options, config));

        services.TryAddScoped<IStampsRateProvider, StampsRateProvider>();

        return services;
    }
}
