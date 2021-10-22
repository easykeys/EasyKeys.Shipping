using EasyKeys.Shipping.PostalAddress;

using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

public static class AddressServiceCollection
{
    /// <summary>
    /// Adds <see cref="IAddressParser"/> to the DI Collection.
    /// </summary>
    /// <param name="services">The DI services.</param>
    /// <returns></returns>
    public static IServiceCollection AddAddressParser(this IServiceCollection services)
    {
        services.TryAddSingleton<IAddressParser, AddressParser>();
        return services;
    }
}
