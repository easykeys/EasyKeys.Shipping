using System.Diagnostics;
using System.Net.Http.Json;

using EasyKeys.Shipping.Abstractions;
using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.FedEx.Abstractions.Options;
using EasyKeys.Shipping.FedEx.AddressValidation.Api.V1;
using EasyKeys.Shipping.FedEx.AddressValidation.Api.V1.Models;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EasyKeys.Shipping.FedEx.AddressValidation.Api.V1.Impl;
public class FedExAddressValidationApiClient : IFedExAddressValidationApiClient, IAddressValidationProvider
{
    private readonly HttpClient _client;
    private readonly FedExApiOptions _options;
    private readonly ILogger<FedExAddressValidationApiClient> _logger;

    public FedExAddressValidationApiClient(
        HttpClient client,
        IOptionsMonitor<FedExApiOptions> optionsMonitor,
        ILogger<FedExAddressValidationApiClient> logger)
    {
        _options = optionsMonitor.CurrentValue;
        _client = client;
        _logger = logger;
    }

    public string Name => nameof(FedExAddressValidationApiClient);

    public async Task<ValidateAddress> ValidateAddressAsync(ValidateAddress request, CancellationToken cancellationToken)
    {
        var watch = ValueStopwatch.StartNew();

        try
        {
            var apiRequest = new AddressValidationRequest
            {
                InEffectAsOfTimestamp = "2019-09-06",
                ValidateAddressControlParameters = new ValidateAddressControlParameters
                {
                    IncludeResolutionTokens = true
                },
                AddressesToValidate = new List<AddressToValidate>
            {
                new AddressToValidate
                {
                    Address = new Models.Address
                    {
                        StreetLines = request.OriginalAddress.GetStreetLines(),
                        City = request.OriginalAddress.City,
                        StateOrProvinceCode = request.OriginalAddress.StateOrProvince,
                        PostalCode = request.OriginalAddress.PostalCode,
                        CountryCode = request.OriginalAddress.CountryCode
                    },
                    ClientReferenceId = request.Id
                }
            }
            };
            var result = await _client.PostAsJsonAsync($"{_options.Url}address/v1/addresses/resolve", apiRequest, cancellationToken);

            result.EnsureSuccessStatusCode();

            var content = await result.Content.ReadAsStringAsync(cancellationToken);

            var response = await result.Content.ReadFromJsonAsync<AddressValidationResponse>(cancellationToken);

            ArgumentNullException.ThrowIfNull(response);

            var lines = response.Output.ResolvedAddresses.FirstOrDefault()?.StreetLinesToken ?? [string.Empty];
            var address1 = lines.FirstOrDefault() ?? string.Empty;
            var address2 = lines.Length > 1 ? lines[1] : string.Empty;

            request.ProposedAddress = new Shipping.Abstractions.Models.Address(
                address1,
                address2,
                response.Output.ResolvedAddresses.FirstOrDefault()?.CityToken?.FirstOrDefault()?.Value ?? string.Empty,
                response.Output.ResolvedAddresses.FirstOrDefault()?.StateOrProvinceCode ?? string.Empty,
                response.Output.ResolvedAddresses.FirstOrDefault()?.PostalCodeToken.Value ?? string.Empty,
                response.Output.ResolvedAddresses.FirstOrDefault()?.CountryCode ?? string.Empty,
                response.Output.ResolvedAddresses.FirstOrDefault()?.Classification == "RESIDENTIAL");

            if (lines.Length == 2)
            {
                request.ProposedAddress.StreetLine2 = lines[1];
            }

            foreach (var a in response.Output.ResolvedAddresses[0].Attributes)
            {
                if (!request.ValidationBag.ContainsKey(a.Key))
                {
                    request.ValidationBag.Add(a.Key, a.Value?.ToString() ?? string.Empty);
                }
            }

            if (response.Output.Alerts != null)
            {
                foreach (var notification in response.Output.Alerts)
                {
                    request.Errors.Add(new Error
                    {
                        Source = notification.AlertType,
                        Number = notification.Code,
                        Description = notification.Message
                    });
                }

                _logger.LogWarning("{providerName} alerts: {errors} ", nameof(FedExAddressValidationApiClient), request.Errors.Select(x => x.Description).Flatten(","));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{providerName} failed", nameof(FedExAddressValidationApiClient));
            request.InternalErrors.Add(ex?.Message ?? $"{nameof(FedExAddressValidationApiClient)} failed");
        }

        _logger.LogDebug("[FedEx][ValidateAddressAsync] completed: {mil}", watch.GetElapsedTime().TotalMilliseconds);
        return request;
    }
}
