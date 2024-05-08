using System.Net.Http.Json;
using EasyKeys.Shipping.FedEx.Abstractions.Models;
using EasyKeys.Shipping.FedEx.Abstractions.Options;
using EasyKeys.Shipping.FedEx.AddressValidation.Client.V1.Models.Request;
using EasyKeys.Shipping.FedEx.AddressValidation.Client.V1.Models.Response;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EasyKeys.Shipping.FedEx.AddressValidation.Client.V1.Impl;

public class FedExAddressValidationClient : IFedexAddressValidationClient
{
    private readonly HttpClient _client;
    private readonly FedExApiOptions _options;
    private ILogger<FedExAddressValidationClient> _logger;

    public FedExAddressValidationClient(
        IOptionsMonitor<FedExApiOptions> optionsMonitor,
        ILogger<FedExAddressValidationClient> logger,
        HttpClient client)
    {
        _options = optionsMonitor.CurrentValue;
        _logger = logger;
        _client = client;
    }

    public async Task<ResponseRoot> ValidateAddressAsync(RequestRoot request, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _client.PostAsJsonAsync($"{_options.Url}address/v1/addresses/resolve", request, _options.JsonSerializerOptions, cancellationToken);

            var content = await result.Content.ReadAsStringAsync(cancellationToken);

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
