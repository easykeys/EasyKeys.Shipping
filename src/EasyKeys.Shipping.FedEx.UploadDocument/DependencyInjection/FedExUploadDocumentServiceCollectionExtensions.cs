using EasyKeys.Shipping.FedEx.Abstractions.Options;
using EasyKeys.Shipping.FedEx.UploadDocument;

namespace Microsoft.Extensions.DependencyInjection;

public static class FedExUploadDocumentServiceCollectionExtensions
{
    /// <summary>
    /// Adds <see cref="IFedExDocumentsProvider"/> implementation.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="sectionName"></param>
    /// <param name="configure"></param>
    /// <returns></returns>
    public static IServiceCollection AddFedExDocumentProvider(
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
}
