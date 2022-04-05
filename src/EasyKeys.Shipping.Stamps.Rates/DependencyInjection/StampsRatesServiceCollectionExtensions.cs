using EasyKeys.Shipping.Stamps.Abstractions.Options;

namespace Microsoft.Extensions.DependencyInjection;

public static class StampsRatesServiceCollectionExtensions
{
    public static IServiceCollection AddStampsRateProvider(
        this IServiceCollection services,
        string sectionName = nameof(StampsOptions),
        Action<StampsOptions, IServiceProvider>? configure = null)
    {
        services.AddStampsClient(
            sectionName,
            configure);

        return services;
    }
}
