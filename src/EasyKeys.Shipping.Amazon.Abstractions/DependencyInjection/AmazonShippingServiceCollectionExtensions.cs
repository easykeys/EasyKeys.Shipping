using EasyKeys.Shipping.Amazon.Abstractions.OpenApis.V2.Shipping;
using EasyKeys.Shipping.Amazon.Abstractions.Options;
using EasyKeys.Shipping.Amazon.Abstractions.Services;
using EasyKeys.Shipping.Amazon.Abstractions.Services.Impl;

namespace Microsoft.Extensions.DependencyInjection;

public static class AmazonShippingServiceCollectionExtensions
{
    public static IServiceCollection AddAmazonShippingClient(
        this IServiceCollection services,
        string sectionName = nameof(AmazonShippingApiOptions),
        Action<AmazonShippingApiOptions, IServiceProvider>? configure = null)
    {
        //services.AddChangeTokenOptions<AmazonShippingApiOptions>(sectionName, null, (options, config) => configure?.Invoke(options, config));
        services.AddSingleton<IAmazonApiAuthenticatorService, AmazonApiAuthenticatorService>();
        // add generated api clients
        // services.AddHttpClient<AuthorizationApi>();
        services.AddHttpClient<AmazonShippingApi>();

        return services;
    }
}
