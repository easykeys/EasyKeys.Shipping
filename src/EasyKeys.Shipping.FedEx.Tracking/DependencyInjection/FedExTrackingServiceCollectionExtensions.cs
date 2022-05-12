using EasyKeys.Shipping.FedEx.Abstractions.Options;
using EasyKeys.Shipping.FedEx.Tracking;

namespace Microsoft.Extensions.DependencyInjection;

public static class FedExTrackingServiceCollectionExtensions
{
    /// <summary>
    /// Adds <see cref="IFedExTrackingProvider"/> implementation.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="sectionName"></param>
    /// <param name="configure"></param>
    /// <returns></returns>
    public static IServiceCollection AddFedExTrackingProvider(
        this IServiceCollection services,
        string sectionName = nameof(FedExOptions),
        Action<FedExOptions, IServiceProvider>? configure = null)
    {
        services.AddChangeTokenOptions<FedExOptions>(sectionName, null, (options, config) => configure?.Invoke(options, config));

        services.AddLogging();

        services.AddFedExClient(sectionName);

        services.AddTransient<IFedExTrackingProvider, FedExTrackingProvider>();

        return services;
    }
}
