using EasyKeys.Shipping.Usps.Abstractions.Options;
using EasyKeys.Shipping.Usps.Tracking;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

using Polly;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class UspsTrackingServiceCollectionExtensions
    {
        /// <summary>
        /// Adds USPS Tracking Http Client <see cref="IUspsTrackingClient"/>.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configureOptions"></param>
        /// <param name="configureClient"></param>
        /// <param name="configurePolicy"></param>
        /// <returns></returns>
        public static IServiceCollection AddUspsTracking(
            this IServiceCollection services,
            Action<UspsOptions, IConfiguration>? configureOptions = null,
            Action<IServiceProvider, HttpClient>? configureClient = null,
            Func<PolicyBuilder<HttpResponseMessage>, IAsyncPolicy<HttpResponseMessage>>? configurePolicy = null)
        {
            services.AddChangeTokenOptions<UspsOptions>(
                    nameof(UspsOptions),
                    null,
                    (o, c) => configureOptions?.Invoke(o, c));

            var builder = services.AddHttpClient<IUspsTrackingClient, UspsTrackingClient>()
                .ConfigureHttpClient((sp, options) =>
                {
                    var opt = sp.GetRequiredService<IOptions<UspsOptions>>();

                    options.BaseAddress = opt.Value.BaseUri;

                    configureClient?.Invoke(sp, options);
                });

            if (configurePolicy != null)
            {
                builder.AddTransientHttpErrorPolicy(configurePolicy);
            }
            else
            {
                builder.AddTransientHttpErrorPolicy(p =>
                {
                    return p.WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)))
                                       .WithPolicyKey($"{nameof(UspsTrackingClient)}-WaitAndRetryAsync");
                });
            }

            return services;
        }
    }
}
