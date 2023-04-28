using EasyKeys.Shipping.Abstractions;
using EasyKeys.Shipping.Stamps.Abstractions.Options;
using EasyKeys.Shipping.Stamps.AddressValidation;

using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

public static class StampsAddressServiceCollectionExtensions
{
    /// <summary>
    /// Adds <see cref="IStampsAddressValidationProvider"/>
    /// and also <see cref="IAddressValidationProvider"/> to the DI container.
    /// </summary>
    /// <param name="services">The DI services.</param>
    /// <param name="sectionName">The name of the section to be used to load configurations.</param>
    /// <param name="configure">The configuration of the options <see cref="StampsOptions"/>.</param>
    /// <returns></returns>
    public static IServiceCollection AddStampsAddressProvider(
        this IServiceCollection services,
        string sectionName = nameof(StampsOptions),
        Action<StampsOptions, IServiceProvider>? configure = null)
    {
        services.AddStampsClient(sectionName, configure);

        services.TryAddTransient<IStampsAddressValidationProvider, StampsAddressValidationProvider>();

        services.AddTransient<IAddressValidationProvider, StampsAddressValidationProvider>();

        return services;
    }
}
