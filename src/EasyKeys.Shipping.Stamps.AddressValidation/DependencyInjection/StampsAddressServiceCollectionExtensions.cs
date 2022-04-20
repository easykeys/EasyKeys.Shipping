using EasyKeys.Shipping.Stamps.Abstractions.Options;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EasyKeys.Shipping.Stamps.AddressValidation.DependencyInjection
{
    public static class StampsAddressServiceCollectionExtensions
    {
        public static IServiceCollection AddStampsAddressProvider(
        this IServiceCollection services,
        string sectionName = nameof(StampsOptions),
        Action<StampsOptions, IServiceProvider>? configure = null)
        {
            services.AddChangeTokenOptions<StampsOptions>(sectionName, null, (options, config) => configure?.Invoke(options, config));

            services.TryAddScoped<IStampsAddressValidationProvider, StampsAddressValidationProvider>();

            return services;
        }
    }
}
