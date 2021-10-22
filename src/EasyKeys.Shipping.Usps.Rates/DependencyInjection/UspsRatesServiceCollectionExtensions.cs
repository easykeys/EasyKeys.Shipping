using EasyKeys.Shipping.Usps.Abstractions.Options;
using EasyKeys.Shipping.Usps.Rates;

namespace Microsoft.Extensions.DependencyInjection;

public static class UspsRatesServiceCollectionExtensions
{
    /// <summary>
    /// Add <see cref="IUspsRateProvider"/> implementation.
    /// </summary>
    /// <param name="services">The DI services.</param>
    /// <param name="sectionName">The configuration section name. The default <see cref="UspsOptions"/> name.</param>
    /// <param name="configure">The configuration of the <see cref="UspsOptions"/> options.</param>
    /// <returns></returns>
    public static IServiceCollection AddUspsRateProvider(
        this IServiceCollection services,
        string sectionName = nameof(UspsOptions),
        Action<UspsOptions, IServiceProvider>? configure = null)
    {
        services.AddChangeTokenOptions<UspsOptions>(sectionName: sectionName, configureAction: (o, s) => configure?.Invoke(o, s));

        services.AddTransient<IUspsRateProvider, UspsRateProvider>();

        // TODO: add resilience to the code
        services.AddHttpClient<IUspsRateProvider, UspsRateProvider>();

        return services;
    }
}
