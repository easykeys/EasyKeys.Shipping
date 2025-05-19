using EasyKeys.Shipping.Amazon.Abstractions.OpenApis.V2.Shipping;
using EasyKeys.Shipping.Amazon.Abstractions.Options;
using EasyKeys.Shipping.Amazon.Abstractions.Services;
using EasyKeys.Shipping.Amazon.Abstractions.Services.Impl;

using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

public static class AmazonShippingServiceCollectionExtensions
{
    /// <summary>
    /// adds the AmazonShippingClient to the DI container.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="sectionName"></param>
    /// <param name="configure"></param>
    /// <returns></returns>
    public static IServiceCollection AddAmazonShippingClient(
        this IServiceCollection services,
        string sectionName = nameof(AmazonShippingApiOptions),
        Action<AmazonShippingApiOptions, IServiceProvider>? configure = null)
    {
        services.AddChangeTokenOptions<AmazonShippingApiOptions>(sectionName, null, (options, config) => configure?.Invoke(options,config));
        services.TryAddSingleton<IAmazonApiAuthenticatorService, AmazonApiAuthenticatorService>();
        // add generated api clients
        // services.AddHttpClient<AuthorizationApi>();
        services.AddHttpClient<AmazonShippingApi>();

        return services;
    }
}
