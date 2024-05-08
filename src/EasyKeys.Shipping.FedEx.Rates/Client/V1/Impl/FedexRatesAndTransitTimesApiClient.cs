using System.Net.Http.Json;

using EasyKeys.Shipping.FedEx.Abstractions.Models;
using EasyKeys.Shipping.FedEx.Abstractions.Options;
using EasyKeys.Shipping.FedEx.Rates.Client.V1.Models.Request;
using EasyKeys.Shipping.FedEx.Rates.Client.V1.Models.Response;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EasyKeys.Shipping.FedEx.Rates.Client.V1.Impl;
public class FedexRatesAndTransitTimesClient : IFedexRatesAndTransitTimesClient
{
    private readonly HttpClient _client;
    private readonly FedExApiOptions _options;
    private readonly ILogger<FedexRatesAndTransitTimesClient> _logger;

    public FedexRatesAndTransitTimesClient(
        IOptionsMonitor<FedExApiOptions> optionsMonitor,
        HttpClient client,
        ILogger<FedexRatesAndTransitTimesClient> logger)
    {
        _options = optionsMonitor.CurrentValue;
        _client = client;
        _logger = logger;
    }

    public async Task<ResponseRoot> GetRatesAsync(RequestRoot request, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _client.PostAsJsonAsync($"{_options.Url}rate/v1/rates/quotes", request, _options.JsonSerializerOptions, cancellationToken);

            var resultObject = await result.Content.ReadFromJsonAsync<ResponseRoot>(_options.JsonSerializerOptions, cancellationToken);

            ArgumentNullException.ThrowIfNull(resultObject, nameof(ResponseRoot));

            return resultObject;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return new ResponseRoot { Errors = { new ApiError { Message = ex.Message } } };
        }
    }
}
