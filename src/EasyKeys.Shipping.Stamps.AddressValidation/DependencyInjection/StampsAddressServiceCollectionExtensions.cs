using EasyKeys.Shipping.Stamps.Abstractions.Options;
using EasyKeys.Shipping.Stamps.AddressValidation;

using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

public static class StampsAddressServiceCollectionExtensions
{
    public static IServiceCollection AddStampsAddressProvider(
    this IServiceCollection services,
    string sectionName = nameof(StampsOptions),
    Action<StampsOptions, IServiceProvider>? configure = null)
    {
        services.AddStampsClient(sectionName, configure);
        services.TryAddScoped<IStampsAddressValidationProvider, StampsAddressValidationProvider>();

        return services;
    }
}
