using System.ServiceModel;

using EasyKeys.Shipping.Stamps.Abstractions.Options;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Polly;

using StampsClient.v111;

namespace EasyKeys.Shipping.Stamps.Abstractions.Services.Impl;
internal sealed class StampsClientService : IStampsClientService
{
    private readonly IStampsClientAuthenticator _stampsClientAuthenticator;
    private readonly ILogger<StampsClientService> _logger;
    private StampsOptions _options;

    private SwsimV111Soap _client;
    private IAsyncPolicy _policy;

    private SemaphoreSlim _mutex = new SemaphoreSlim(1);

    private object _credentials;

    public StampsClientService(
        IOptionsMonitor<StampsOptions> optionsMonitor,
        IStampsClientAuthenticator stampsClientAuthenticator,
        ILoggerFactory loggerFactory,
        ILogger<StampsClientService> logger)
    {
        _options = optionsMonitor.CurrentValue;

        optionsMonitor.OnChange(x => _options = x);
        _stampsClientAuthenticator = stampsClientAuthenticator;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _client = new SwsimV111SoapClient(
             new BasicHttpsBinding(BasicHttpsSecurityMode.Transport)
             {
                 MaxReceivedMessageSize = int.MaxValue,
             },
             new EndpointAddress(_options.Url));

        _policy = Policies.GetWaitRetryAsyc(stampsClientAuthenticator, loggerFactory);

        // this can be used thru the rest of the instance.
        _credentials = new Credentials()
        {
            IntegrationID = new Guid(_options.IntegrationId),
            Username = _options.UserName,
            Password = _options.Password
        };
    }

    public SwsimV111Soap CreateClient()
    {
        return _client;
    }

    public async Task<CleanseAddressResponse> CleanseAddressAsync(CleanseAddressRequest request, CancellationToken cancellationToken)
    {
        var response = await _policy.ExecuteAsync(
            async (ctx, cts) =>
            {
                await _mutex.WaitAsync();
                try
                {
                    request.Item = _options.UseAuthenticator ? _stampsClientAuthenticator.GetToken() : _credentials;

                    var respo = await _client.CleanseAddressAsync(request);

                    if (_options.UseAuthenticator)
                    {
                        _stampsClientAuthenticator.SetToken(respo.Authenticator);
                    }

                    return respo;
                }
                finally
                {
                    _mutex.Release();
                }
            },
            context: new Context(),
            cancellationToken: cancellationToken);

        return response;
    }

    public async Task<GetRatesResponse> GetRatesAsync(GetRatesRequest request, CancellationToken cancellationToken)
    {
        var response = await _policy.ExecuteAsync(
              async (ctx, cts) =>
              {
                  await _mutex.WaitAsync();

                  try
                  {
                      request.Item = _options.UseAuthenticator ? _stampsClientAuthenticator.GetToken() : _credentials;

                      var respo = await _client.GetRatesAsync(request);

                      if (_options.UseAuthenticator)
                      {
                          _stampsClientAuthenticator.SetToken(respo.Authenticator);
                      }

                      return respo;
                  }
                  finally
                  {
                      _mutex.Release();
                  }
              },
              context: new Context(),
              cancellationToken: cancellationToken);

        return response;
    }

    public async Task<CreateIndiciumResponse> CreateIndiciumAsync(CreateIndiciumRequest request, CancellationToken cancellationToken)
    {
        var response = await _policy.ExecuteAsync(
              async (ctx, cts) =>
              {
                  await _mutex.WaitAsync();

                  try
                  {
                      request.Item = _options.UseAuthenticator ? _stampsClientAuthenticator.GetToken() : _credentials;

                      var respo = await _client.CreateIndiciumAsync(request);

                      if (_options.UseAuthenticator)
                      {
                          _stampsClientAuthenticator.SetToken(respo.Authenticator);
                      }

                      return respo;
                  }
                  finally
                  {
                      _mutex.Release();
                  }
              },
              context: new Context(),
              cancellationToken: cancellationToken);

        return response;
    }

    public async Task<CancelIndiciumResponse> CancelIndiciumAsync(CancelIndiciumRequest request, CancellationToken cancellationToken)
    {
        var response = await _policy.ExecuteAsync(
              async (ctx, cts) =>
              {
                  await _mutex.WaitAsync();

                  try
                  {
                      request.Item = _options.UseAuthenticator ? _stampsClientAuthenticator.GetToken() : _credentials;

                      var respo = await _client.CancelIndiciumAsync(request);

                      if (_options.UseAuthenticator)
                      {
                          _stampsClientAuthenticator.SetToken(respo.Authenticator);
                      }

                      return respo;
                  }
                  finally
                  {
                      _mutex.Release();
                  }
              },
              context: new Context(),
              cancellationToken: cancellationToken);

        return response;
    }

    public async Task<TrackShipmentResponse> TrackShipmentAsync(TrackShipmentRequest request, CancellationToken cancellationToken)
    {
        var response = await _policy.ExecuteAsync(
              async (ctx, cts) =>
              {
                  await _mutex.WaitAsync();

                  try
                  {
                      request.Item = _options.UseAuthenticator ? _stampsClientAuthenticator.GetToken() : _credentials;

                      var respo = await _client.TrackShipmentAsync(request);

                      if (_options.UseAuthenticator)
                      {
                          _stampsClientAuthenticator.SetToken(respo.Authenticator);
                      }

                      return respo;
                  }
                  finally
                  {
                      _mutex.Release();
                  }
              },
              context: new Context(),
              cancellationToken: cancellationToken);

        return response;
    }
}
