using System.Diagnostics;

using EasyKeys.Shipping.Abstractions;
using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.FedEx.Abstractions.OpenApis.V1.AddressValidation;
using EasyKeys.Shipping.FedEx.Abstractions.Services;

using Microsoft.Extensions.Logging;

using Address = EasyKeys.Shipping.FedEx.Abstractions.OpenApis.V1.AddressValidation.Address;

namespace EasyKeys.Shipping.FedEx.AddressValidation.RestApi.Impl;

/// <summary>
/// This validates recipient address information and identifies it as either business or residential.Uses Fedex REST Api Address Validation Client V1.
/// </summary>
public class FedExAddressValidationProvider : IFedExAddressValidationProvider, IAddressValidationProvider
{
    private readonly AddressValidationApi _client;
    private readonly IFedexApiAuthenticatorService _authService;
    private readonly ILogger<FedExAddressValidationProvider> _logger;

    public FedExAddressValidationProvider(
        AddressValidationApi client,
        IFedexApiAuthenticatorService authService,
        ILogger<FedExAddressValidationProvider> logger)
    {
        _authService = authService;
        _client = client;
        _logger = logger;
    }

    public string Name => nameof(FedExAddressValidationProvider);

    public async Task<ValidateAddress> ValidateAddressAsync(ValidateAddress request, CancellationToken cancellationToken = default)
    {
        var watch = ValueStopwatch.StartNew();

        try
        {
            var apiRequest = new Full_Schema_Validate_Address
            {
                InEffectAsOfTimestamp = "2019-09-06",
                ValidateAddressControlParameters = new AddressResolutionControlParameters
                {
                    IncludeResolutionTokens = true
                },
                AddressesToValidate = new List<ResolveContactAndAddress>
                {
                    new ResolveContactAndAddress
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
            var token = await _authService.GetTokenAsync(cancellationToken);
            var response = await _client.Validate_AddressAsync(
                apiRequest,
                Guid.NewGuid().ToString(),
                "application/json",
                "en_US",
                token,
                cancellationToken);

            var lines = response.Output?.ResolvedAddresses?.FirstOrDefault()?.StreetLinesToken ?? [string.Empty];
            var address1 = lines.FirstOrDefault() ?? string.Empty;
            var address2 = lines.Count > 1 ? lines.Last() : string.Empty;

            request.ProposedAddress = new Shipping.Abstractions.Models.Address(
                address1,
                address2,
                response.Output?.ResolvedAddresses?.FirstOrDefault()?.CityToken?.FirstOrDefault()?.Value ?? string.Empty,
                response.Output?.ResolvedAddresses?.FirstOrDefault()?.StateOrProvinceCode ?? string.Empty,
                response.Output?.ResolvedAddresses?.FirstOrDefault()?.PostalCodeToken?.Value ?? string.Empty,
                response.Output?.ResolvedAddresses?.FirstOrDefault()?.CountryCode ?? string.Empty,
                response.Output?.ResolvedAddresses?.FirstOrDefault()?.Classification == ResolvedAddressClassification.RESIDENTIAL);

            var dictionary = ConvertAttributesToDictionary(response.Output?.ResolvedAddresses?.FirstOrDefault()?.Attributes ?? new Attributes());

            foreach (var a in dictionary)
            {
                if (!request.ValidationBag.ContainsKey(a.Key))
                {
                    request.ValidationBag.Add(a.Key, a.Value?.ToString() ?? string.Empty);
                }
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

    private Dictionary<string, string> ConvertAttributesToDictionary(Attributes attributes)
    {
        var dictionary = new Dictionary<string, string>
        {
            // Adding boolean properties as strings
            { nameof(attributes.POBox), attributes.POBox.ToString() },
            { nameof(attributes.POBoxOnlyZIP), attributes.POBoxOnlyZIP.ToString() },
            { nameof(attributes.SplitZip), attributes.SplitZip.ToString() },
            { nameof(attributes.SuiteRequiredButMissing), attributes.SuiteRequiredButMissing.ToString() },
            { nameof(attributes.InvalidSuiteNumber), attributes.InvalidSuiteNumber.ToString() },
            { nameof(attributes.DPV), attributes.DPV.ToString() },
            { nameof(attributes.CountrySupported), attributes.CountrySupported.ToString() },
            { nameof(attributes.ValidlyFormed), attributes.ValidlyFormed.ToString() },
            { nameof(attributes.Matched), attributes.Matched.ToString() },
            { nameof(attributes.Resolved), attributes.Resolved.ToString() },
            { nameof(attributes.Inserted), attributes.Inserted.ToString() },
            { nameof(attributes.MultiUnitBase), attributes.MultiUnitBase.ToString() },
            { nameof(attributes.ZIP11Match), attributes.ZIP11Match.ToString() },
            { nameof(attributes.ZIP4Match), attributes.ZIP4Match.ToString() },
            { nameof(attributes.UniqueZIP), attributes.UniqueZIP.ToString() },
            { nameof(attributes.StreetAddress), attributes.StreetAddress.ToString() },
            { nameof(attributes.RRConversion), attributes.RRConversion.ToString() },
            { nameof(attributes.ValidMultiUnit), attributes.ValidMultiUnit.ToString() },
            { nameof(attributes.MultipleMatches), attributes.MultipleMatches.ToString() },

            // Adding string properties directly
            { nameof(attributes.ResolutionInput), attributes.ResolutionInput ?? string.Empty },
            { nameof(attributes.ResolutionMethod), attributes.ResolutionMethod ?? string.Empty },
            { nameof(attributes.DataVintage), attributes.DataVintage ?? string.Empty },
            { nameof(attributes.MatchSource), attributes.MatchSource ?? string.Empty },
            { nameof(attributes.AddressType), attributes.AddressType ?? string.Empty },
            { nameof(attributes.AddressPrecision), attributes.AddressPrecision ?? string.Empty }
        };

        // Additional properties can also be added if needed
        foreach (var additional in attributes.AdditionalProperties)
        {
            dictionary.Add(additional.Key, additional.Value?.ToString() ?? string.Empty);
        }

        return dictionary;
    }
}
