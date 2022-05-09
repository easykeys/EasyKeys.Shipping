using EasyKeys.Shipping.Stamps.Abstractions.Options;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EasyKeys.Shipping.Stamps.Tracking.DependencyInjection
{
    public static class StampsTrackingServiceCollectionExtensions
    {
        /// <summary>
        /// Adds Stamps.om Tracking provider.
        /// </summary>
        /// <param name="services">The DI services.</param>
        /// <param name="sectionName">The name of the configuraiton section.</param>
        /// <param name="configure">The action to configure <see cref="StampsOptions"/>.</param>
        /// <returns></returns>
        public static IServiceCollection AddStampsTrackingProvider(
            this IServiceCollection services,
            string sectionName = nameof(StampsOptions),
            Action<StampsOptions, IServiceProvider>? configure = null)
        {
            services.AddStampsClient(sectionName, configure);

            services.TryAddScoped<IStampsTrackingProvider, StampsTrackingProvider>();

            return services;
        }
    }
}
