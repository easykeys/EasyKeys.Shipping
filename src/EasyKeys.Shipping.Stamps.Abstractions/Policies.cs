using System.ServiceModel;

using Microsoft.Extensions.Logging;

using Polly;
using Polly.Timeout;

namespace EasyKeys.Shipping.Stamps.Abstractions.Services.Impl;

public static class Policies
{
    public static IAsyncPolicy GetWaitRetryAsyc(
        IStampsClientAuthenticator client,
        ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger("Polices");

        var timeoutPolicy = Policy.TimeoutAsync(30, TimeoutStrategy.Pessimistic);

        var jitterer = new Random();

        return Policy
              .Handle<FaultException>(x => x.Message == "Conversation out-of-sync.")
              .Or<FaultException>(x => x.Message == "Invalid conversation token.")
              .Or<FaultException>(x => x.Message == "Authentication failed.")
              .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: (retryAttempt, context) =>
                {
                    return TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                          + TimeSpan.FromMilliseconds(jitterer.Next(0, 50));
                },
                onRetryAsync: async (ex, count, context) =>
                {
                    logger.LogWarning("[Stamps.com] - Clearing Authorization Token, {exType} : {message} was thrown.", ex.GetType().ToString(), ex.Message);

                    client.ClearTokens();

                    await Task.CompletedTask;
                })
                .WrapAsync(timeoutPolicy);
    }
}
