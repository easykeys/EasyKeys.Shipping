using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EasyKeys.Shipping.Amazon.Abstractions.Options;

using Microsoft.Extensions.DependencyInjection;

namespace EasyKeys.Shipping.Amazon.Rates.DependencyInjection;

public static class AmazonRatesServiceCollectionExtensions
{
    public static IServiceCollection AddRestApiAmazonRateProvider(
        this IServiceCollection services,
        string sectionName = nameof(AmazonShippingApiOptions),
        Action<AmazonShippingApiOptions, IServiceProvider>? configOptions = null)
    {
        services.AddLogging();

        services.AddAmazonShippingClient();

        services.AddTransient<IAmazonShippingRateProvider, AmazonShippingRateProvider>();

        return services;
    }
}
