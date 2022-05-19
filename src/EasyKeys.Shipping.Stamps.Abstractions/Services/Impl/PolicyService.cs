using System.ServiceModel;

using Microsoft.Extensions.Logging;

using Polly;
using Polly.Timeout;

namespace EasyKeys.Shipping.Stamps.Abstractions.Services.Impl
{
    public class PolicyService : IPolicyService
    {
        private readonly IStampsClientService _stampsClient;
        private readonly ILogger<PolicyService> _logger;

        public PolicyService(IStampsClientService stampsClient, ILogger<PolicyService> logger)
        {
            _stampsClient = stampsClient;
            _logger = logger;
        }

        public IAsyncPolicy GetRetryWithRefreshToken(CancellationToken cancellationToken)
        {
            var timeoutPolicy = Policy.TimeoutAsync(10, TimeoutStrategy.Pessimistic);

            var jitterer = new Random();

            return Policy
                  .Handle<FaultException>(x => x.Message == "Conversation out-of-sync.")
                  .WaitAndRetryAsync(
                    retryCount: 1,
                    sleepDurationProvider: (retryAttempt, context) =>
                    {
                        return TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                              + TimeSpan.FromMilliseconds(jitterer.Next(0, 200));
                    },
                    onRetry: async (ex, count, context) =>
                    {
                        _logger.LogError("Refreshing Token, {ex} : {message} was thrown.", ex.GetType().ToString(), ex.Message);
                        await _stampsClient.RefreshTokenAsync(cancellationToken);
                    })
                    .WrapAsync(timeoutPolicy);
        }
    }
}
