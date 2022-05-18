using Polly;

namespace EasyKeys.Shipping.Stamps.Abstractions.Services
{
    public interface IPolicyService
    {
        IAsyncPolicy GetRetryWithRefreshToken(CancellationToken cancellationToken);
    }
}
