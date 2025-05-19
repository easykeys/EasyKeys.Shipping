using EasyKeys.Shipping.Abstractions;
using EasyKeys.Shipping.DHL.Abstractions;
using EasyKeys.Shipping.DHL.Abstractions.DependencyInjection;
using EasyKeys.Shipping.DHL.Abstractions.OpenApis.V2.Express;
using EasyKeys.Shipping.DHL.Abstractions.Options;

using Microsoft.Extensions.DependencyInjection;

namespace EasyKeys.Shipping.DHL.AddressValidation.DependencyInjection;

public static class DHLAddressValidationServiceCollectionExtensions
{
    /// <summary>
    /// adds the DHLExpressClient to the DI container. required for usage of all dhl express shipping services.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="sectionName"></param>
    /// <param name="configure"></param>
    /// <returns></returns>
    public static IServiceCollection AddDHLExpressAddressValidationProvider(
        this IServiceCollection services,
        string sectionName = nameof(DHLExpressApiOptions),
        Action<DHLExpressApiOptions, IServiceProvider>? configure = null)
    {
        services.AddDHLExpressClient();

        services.AddTransient<IDHLExpressAddressValidationProvider, DHLExpressAddressValidationProvider>();

        services.AddTransient<IAddressValidationProvider, DHLExpressAddressValidationProvider>();

        return services;
    }
}
