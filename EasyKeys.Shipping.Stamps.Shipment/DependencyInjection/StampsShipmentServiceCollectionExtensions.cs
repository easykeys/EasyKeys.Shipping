
using EasyKeys.Shipping.Stamps.Abstractions.Options;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EasyKeys.Shipping.Stamps.Shipment.DependencyInjection
{
    public static class StampsShipmentServiceCollectionExtensions
    {
        public static IServiceCollection AddStampsShipmentProvider(
        this IServiceCollection services,
        string sectionName = nameof(StampsOptions),
        Action<StampsOptions, IServiceProvider>? configure = null)
        {
            services.AddChangeTokenOptions<StampsOptions>(sectionName, null, (options, config) => configure?.Invoke(options, config));

            services.TryAddScoped<IStampsShipmentProvider, StampsShipmentProvider>();

            return services;
        }
    }
}
