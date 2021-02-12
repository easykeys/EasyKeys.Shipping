using AddressValidationClient.v4;

using EasyKeys.Shipping.FedEx.Abstractions.Options;
using EasyKeys.Shipping.FedEx.AddressValidation;

using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AddressValidationServiceExtensions
    {
        public static IServiceCollection AddFedExAddressValidation(this IServiceCollection services, string sectionName = "")
        {
            if (string.IsNullOrEmpty(sectionName))
            {
                sectionName = nameof(FedExOptions);
            }

            services.AddChangeTokenOptions<FedExOptions>(sectionName, null, (options) => { });

            services.AddTransient<IValidationClient, ValidationClient>();

            return services;
        }
    }
}
