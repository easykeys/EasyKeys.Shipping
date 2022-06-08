using EasyKeys.Shipping.Stamps.Abstractions.Options;
using EasyKeys.Shipping.Stamps.Rates;

using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

public static class StampsRatesServiceCollectionExtensions
{
    /// <summary>
    /// Adds Stamps.com wcf rates provider.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="sectionName"></param>
    /// <param name="configure"></param>
    /// <returns></returns>
    public static IServiceCollection AddStampsRateProvider(
        this IServiceCollection services,
        string sectionName = nameof(StampsOptions),
        Action<StampsOptions, IServiceProvider>? configure = null)
    {
        services.AddStampsClient(sectionName, configure);

        services.TryAddScoped<IStampsRateProvider, StampsRateProvider>();

        return services;
    }
}
