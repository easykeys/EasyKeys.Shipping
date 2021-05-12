using System;

using EasyKeys.Shipping.FedEx.Abstractions.Options;
using EasyKeys.Shipping.FedEx.AddressValidation;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AddressValidationServiceExtensions
    {
        /// <summary>
        /// Adds <see cref="IFedExAddressValidationProvider"/> instance to the DI container.
        /// </summary>
        /// <param name="services">The DI services.</param>
        /// <param name="sectionName">The section name for the configuration. The default is <see cref="FedExOptions"/>.</param>
        /// <param name="configOptions">The configuration for the <see cref="FedExOptions"/>. The default is null.</param>
        /// <returns></returns>
        public static IServiceCollection AddFedExAddressValidation(
            this IServiceCollection services,
            string sectionName = nameof(FedExOptions),
            Action<FedExOptions, IServiceProvider>? configOptions = null)
        {
            services.AddChangeTokenOptions<FedExOptions>(
                sectionName: sectionName,
                configureAction: (options, sp) => configOptions?.Invoke(options, sp));

            services.AddLogging();
            services.AddTransient<IFedExAddressValidationProvider, FedExAddressValidationProvider>();

            return services;
        }
    }
}
