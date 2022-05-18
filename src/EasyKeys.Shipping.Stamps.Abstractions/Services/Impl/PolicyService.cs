using System.ServiceModel;

using Polly;
using Polly.Timeout;

namespace EasyKeys.Shipping.Stamps.Abstractions.Services.Impl
{
    public class PolicyService : IPolicyService
    {
        private readonly IStampsClientService _stampsClient;

        public PolicyService(IStampsClientService stampsClient)
        {
            _stampsClient = stampsClient;
        }

        public IAsyncPolicy GetRetryWithRefreshToken(CancellationToken cancellationToken)
        {
            var timeoutPolicy = Policy.TimeoutAsync(3, TimeoutStrategy.Pessimistic);

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
                        await _stampsClient.RefreshTokenAsync(cancellationToken);
                    })
                    .WrapAsync(timeoutPolicy);
        }
    }
}
