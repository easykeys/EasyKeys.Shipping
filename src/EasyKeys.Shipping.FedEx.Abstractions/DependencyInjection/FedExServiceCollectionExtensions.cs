using EasyKeys.Shipping.FedEx.Abstractions.Options;
using EasyKeys.Shipping.FedEx.Abstractions.Services;
using EasyKeys.Shipping.FedEx.Abstractions.Services.Impl;

using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

public static class FedExServiceCollectionExtensions
{
    /// <summary>
    /// Adds FedEx Client Service with configuration options <see cref="FedExOptions"/>.
    /// </summary>
    /// <param name="services">The DI services.</param>
    /// <param name="sectionName">The section name for the options.</param>
    /// <param name="configure">The configuration of options.</param>
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
