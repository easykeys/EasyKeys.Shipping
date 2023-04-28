using EasyKeys.Shipping.Stamps.Abstractions.Options;
using EasyKeys.Shipping.Stamps.Abstractions.Services;
using EasyKeys.Shipping.Stamps.Abstractions.Services.Impl;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Polly;

using StampsClient.v111;

namespace EasyKeysShipping.UnitTest.Stubs
{
    public class StampsClientServiceMock : IStampsClientService
    {
        private readonly IStampsClientAuthenticator _stampsClientAuthenticator;
        private readonly ILogger<StampsClientServiceMock> _logger;
        private StampsOptions _options;

        private SwsimV111Soap _client;
        private IAsyncPolicy _policy;

        private SemaphoreSlim _mutex = new SemaphoreSlim(1);

        public StampsClientServiceMock(
            IOptionsMonitor<StampsOptions> optionsMonitor,
            IStampsClientAuthenticator stampsClientAuthenticator,
            SwsimV111Soap client,
            ILoggerFactory loggerFactory,
            ILogger<StampsClientServiceMock> logger)
        {
            _options = optionsMonitor.CurrentValue;

            optionsMonitor.OnChange(x => _options = x);
            _stampsClientAuthenticator = stampsClientAuthenticator;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client;
            _policy = Policies.GetWaitRetryAsyc(stampsClientAuthenticator, loggerFactory);
        }

        public SwsimV111Soap CreateClient()
        {
            return _client;
        }

        public async Task<CleanseAddressResponse> CleanseAddressAsync(CleanseAddressRequest request, CancellationToken cancellationToken)
        {
            var respone = await _policy.ExecuteAsync(
                async (ctx, cts) =>
                {
                    await _mutex.WaitAsync();
                    try
                    {
                        request.Item = _stampsClientAuthenticator.GetToken();

                        var respo = await _client.CleanseAddressAsync(request);
                        _stampsClientAuthenticator.SetToken(respo.Authenticator);

                        return respo;
                    }
                    finally
                    {
                        _mutex.Release();
                    }
                },
                context: new Context(),
                cancellationToken: cancellationToken);

            return respone;
        }

        public async Task<GetRatesResponse> GetRatesAsync(GetRatesRequest request, CancellationToken cancellationToken)
        {
            var respone = await _policy.ExecuteAsync(
                  async (ctx, cts) =>
                  {
                      await _mutex.WaitAsync();

                      try
                      {
                          request.Item = _stampsClientAuthenticator.GetToken();

                          var respo = await _client.GetRatesAsync(request);
                          _stampsClientAuthenticator.SetToken(respo.Authenticator);

                          return respo;
                      }
                      finally
                      {
                          _mutex.Release();
                      }
                  },
                  context: new Context(),
                  cancellationToken: cancellationToken);

            return respone;
        }

        public async Task<CreateIndiciumResponse> CreateIndiciumAsync(CreateIndiciumRequest request, CancellationToken cancellationToken)
        {
            var respone = await _policy.ExecuteAsync(
                  async (ctx, cts) =>
                  {
                      await _mutex.WaitAsync();

                      try
                      {
                          request.Item = _stampsClientAuthenticator.GetToken();
                          var respo = await _client.CreateIndiciumAsync(request);
                          _stampsClientAuthenticator.SetToken(respo.Authenticator);
                          return respo;
                      }
                      finally
                      {
                          _mutex.Release();
                      }
                  },
                  context: new Context(),
                  cancellationToken: cancellationToken);

            return respone;
        }

        public async Task<CancelIndiciumResponse> CancelIndiciumAsync(CancelIndiciumRequest request, CancellationToken cancellationToken)
        {
            var respone = await _policy.ExecuteAsync(
                  async (ctx, cts) =>
                  {
                      await _mutex.WaitAsync();

                      try
                      {
                          request.Item = _stampsClientAuthenticator.GetToken();
                          var respo = await _client.CancelIndiciumAsync(request);
                          _stampsClientAuthenticator.SetToken(respo.Authenticator);
                          return respo;
                      }
                      finally
                      {
                          _mutex.Release();
                      }
                  },
                  context: new Context(),
                  cancellationToken: cancellationToken);

            return respone;
        }

        public async Task<TrackShipmentResponse> TrackShipmentAsync(TrackShipmentRequest request, CancellationToken cancellationToken)
        {
            var respone = await _policy.ExecuteAsync(
                  async (ctx, cts) =>
                  {
                      await _mutex.WaitAsync();

                      try
                      {
                          request.Item = _stampsClientAuthenticator.GetToken();
                          var respo = await _client.TrackShipmentAsync(request);
                          _stampsClientAuthenticator.SetToken(respo.Authenticator);
                          return respo;
                      }
                      finally
                      {
                          _mutex.Release();
                      }
                  },
                  context: new Context(),
                  cancellationToken: cancellationToken);

            return respone;
        }
    }
}
