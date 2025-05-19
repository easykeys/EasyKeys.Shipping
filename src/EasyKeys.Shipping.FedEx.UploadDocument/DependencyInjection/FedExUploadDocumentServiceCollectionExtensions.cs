using EasyKeys.Shipping.FedEx.Abstractions.Options;
using EasyKeys.Shipping.FedEx.UploadDocument;
using EasyKeys.Shipping.FedEx.UploadDocument.WebServices.Impl;

namespace Microsoft.Extensions.DependencyInjection;

public static class FedExUploadDocumentServiceCollectionExtensions
{
    /// <summary>
    /// Adds SOAP web services <see cref="IFedExDocumentsProvider"/> implementation.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="sectionName"></param>
    /// <param name="configure"></param>
    /// <returns></returns>
    public static IServiceCollection AddWebServicesFedExDocumentProvider(
        this IServiceCollection services,
        string sectionName = nameof(FedExOptions),
        Action<FedExOptions, IServiceProvider>? configure = null)
    {
        services.AddChangeTokenOptions<FedExOptions>(sectionName, null, (options, config) => configure?.Invoke(options, config));

        services.AddLogging();

        services.AddFedExClient(sectionName);

        services.AddTransient<IFedExDocumentsProvider, FedExDocumentProvider>();

        return services;
    }

    /// <summary>
    /// Adds REST API <see cref="FedExServiceCollectionExtensions.AddFedExApiClients" />,  <see cref="IFedExDocumentsProvider" />.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="sectionName"></param>
    /// <param name="configOptions"></param>
    /// <returns></returns>
    public static IServiceCollection AddRestApiFedExDocumentProvider(
        this IServiceCollection services,
        string sectionName = nameof(FedExApiOptions),
        Action<FedExApiOptions, IServiceProvider>? configOptions = null)
    {
        services.AddFedExApiClients();

        services.AddTransient<IFedExDocumentsProvider, EasyKeys.Shipping.FedEx.UploadDocument.RestApi.Impl.FedexDocumentProvider>();

        return services;
    }
}
