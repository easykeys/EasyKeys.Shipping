using EasyKeys.Shipping.PostalAddress;

using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AddressServiceCollection
    {
        public static IServiceCollection AddAddressParser(this IServiceCollection services)
        {
            services.TryAddSingleton<IAddressParser, AddressParser>();
            return services;
        }
    }
}
