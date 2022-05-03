
using EasyKeys.Shipping.FedEx.Abstractions.Options;
using EasyKeys.Shipping.FedEx.Abstractions.Services;
using EasyKeys.Shipping.FedEx.Abstractions.Services.Impl;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EasyKeys.Shipping.FedEx.Abstractions.DependencyInjection
{
    public static class FedExServiceCollectionExtensions
    {
        /// <summary>
        /// Adds Stamps.com authentication service.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="sectionName"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static IServiceCollection AddFedExClient(
            this IServiceCollection services,
            string sectionName = nameof(FedExOptions),
            Action<FedExOptions, IServiceProvider>? configure = null)
        {
            services.AddChangeTokenOptions<FedExOptions>(sectionName, null, (options, config) => configure?.Invoke(options, config));

            services.TryAddScoped<IFedExClientService, FedExClientService>();

            return services;
        }
    }
}
