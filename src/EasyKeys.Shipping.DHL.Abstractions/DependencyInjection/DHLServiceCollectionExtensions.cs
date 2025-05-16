using EasyKeys.Shipping.DHL.Abstractions.OpenApis.V2.Express;
using EasyKeys.Shipping.DHL.Abstractions.Options;

using Microsoft.Extensions.DependencyInjection;

namespace EasyKeys.Shipping.DHL.Abstractions.DependencyInjection;

public static class DHLServiceCollectionExtensions
{
    /// <summary>
    /// adds the DHLExpressClient to the DI container. required for usage of all dhl express shipping services.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="sectionName"></param>
    /// <param name="configure"></param>
    /// <returns></returns>
    public static IServiceCollection AddDHLExpressClient(
        this IServiceCollection services,
        string sectionName = nameof(DHLExpressApiOptions),
        Action<DHLExpressApiOptions, IServiceProvider>? configure = null)
    {
        services.AddChangeTokenOptions<DHLExpressApiOptions>(sectionName, null, (options, config) => configure?.Invoke(options, config));

        services.AddHttpClient<DHLExpressApi>();

        services.AddSingleton<IPaperlessEligibilityService, DHLExpressPaperlessEligibilityService>();

        return services;
    }
}
