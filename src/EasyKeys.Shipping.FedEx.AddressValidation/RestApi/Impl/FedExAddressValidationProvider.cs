using System.Diagnostics;

using EasyKeys.Shipping.Abstractions;
using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.FedEx.Abstractions.Options;
using EasyKeys.Shipping.FedEx.AddressValidation.Client.V1;
using EasyKeys.Shipping.FedEx.AddressValidation.Client.V1.Models.Request;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Address = EasyKeys.Shipping.FedEx.AddressValidation.Client.V1.Models.Request.Address;
using AddressToValidate = EasyKeys.Shipping.FedEx.AddressValidation.Client.V1.Models.Request.AddressesToValidate;

namespace EasyKeys.Shipping.FedEx.AddressValidation.RestApi.Impl;

/// <summary>
/// This validates recipient address information and identifies it as either business or residential.Uses Fedex REST Api Address Validation Client V1.
/// </summary>
public class FedExAddressValidationProvider : IFedExAddressValidationProvider, IAddressValidationProvider
{
    private readonly IFedexAddressValidationClient _client;
    private readonly FedExApiOptions _options;
    private readonly ILogger<FedExAddressValidationProvider> _logger;

    public FedExAddressValidationProvider(
        IFedexAddressValidationClient client,
        IOptionsMonitor<FedExApiOptions> optionsMonitor,
        ILogger<FedExAddressValidationProvider> logger)
    {
        _options = optionsMonitor.CurrentValue;
        _client = client;
        _logger = logger;
    }

    public string Name => nameof(FedExAddressValidationProvider);

    public async Task<ValidateAddress> ValidateAddressAsync(ValidateAddress request, CancellationToken cancellationToken = default)
    {
        var watch = ValueStopwatch.StartNew();

        try
        {
            var apiRequest = new RequestRoot
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
                        Address = new Address
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
            var response = await _client.ValidateAddressAsync(apiRequest, cancellationToken);

            foreach (var error in response.Errors)
            {
                request.Errors.Add(new Error
                {
                    Description = error.Message
                });
            }

            if (request.Errors.Any() || response.Output == null)
            {
                return request;
            }

            var lines = response.Output.ResolvedAddresses?.FirstOrDefault()?.StreetLinesToken ?? [string.Empty];
            var address1 = lines.FirstOrDefault() ?? string.Empty;
            var address2 = lines.Length > 1 ? lines[1] : string.Empty;

            request.ProposedAddress = new Shipping.Abstractions.Models.Address(
                address1,
                address2,
                response.Output.ResolvedAddresses?.FirstOrDefault()?.CityToken?.FirstOrDefault()?.Value ?? string.Empty,
                response.Output.ResolvedAddresses?.FirstOrDefault()?.StateOrProvinceCode ?? string.Empty,
                response.Output.ResolvedAddresses?.FirstOrDefault()?.PostalCodeToken?.Value ?? string.Empty,
                response.Output.ResolvedAddresses?.FirstOrDefault()?.CountryCode ?? string.Empty,
                response.Output.ResolvedAddresses?.FirstOrDefault()?.Classification == "RESIDENTIAL");

            if (lines.Length == 2)
            {
                request.ProposedAddress.StreetLine2 = lines[1];
            }

            foreach (var a in response.Output.ResolvedAddresses?.FirstOrDefault()?.Attributes ?? new ())
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

                _logger.LogWarning("{providerName} alerts: {errors} ", nameof(FedExAddressValidationProvider), request.Errors.Select(x => x.Description).Flatten(","));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{providerName} failed", nameof(FedExAddressValidationProvider));
            request.InternalErrors.Add(ex?.Message ?? $"{nameof(FedExAddressValidationProvider)} failed");
        }

        _logger.LogDebug("[FedEx][ValidateAddressAsync] completed: {mil}", watch.GetElapsedTime().TotalMilliseconds);
        return request;
    }
}
