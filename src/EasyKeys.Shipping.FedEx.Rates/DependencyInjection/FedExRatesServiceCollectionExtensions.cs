using EasyKeys.Shipping.FedEx.Abstractions.Options;
using EasyKeys.Shipping.FedEx.Rates;

namespace Microsoft.Extensions.DependencyInjection;

public static class FedExRatesServiceCollectionExtensions
{
    /// <summary>
    /// Adds <see cref="IFedExRateProvider"/> implementation.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="sectionName"></param>
    /// <param name="configure"></param>
    /// <returns></returns>
    public static IServiceCollection AddFedExRateProvider(
        this IServiceCollection services,
        string sectionName = nameof(FedExOptions),
        Action<FedExOptions, IServiceProvider>? configure = null)
    {
        services.AddChangeTokenOptions<FedExOptions>(sectionName, null, (options, config) => configure?.Invoke(options, config));

        services.AddLogging();

        services.AddTransient<IFedExRateProvider, FedExRateProvider>();

        return services;
    }
}
