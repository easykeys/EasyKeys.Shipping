using System.Diagnostics;

using EasyKeys.Shipping.Abstractions;
using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.DHL.Abstractions.OpenApis.V2.Express;

using Microsoft.Extensions.Logging;

namespace EasyKeys.Shipping.DHL.Abstractions;

public class DHLExpressAddressValidationProvider : IDHLExpressAddressValidationProvider, IAddressValidationProvider
{
    private readonly DHLExpressApi _dhlExpressApi;
    private readonly ILogger<DHLExpressAddressValidationProvider> _logger;

    public DHLExpressAddressValidationProvider(
    DHLExpressApi dhlExpressApi,
    ILogger<DHLExpressAddressValidationProvider> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _dhlExpressApi = dhlExpressApi ?? throw new ArgumentNullException(nameof(dhlExpressApi));
    }

    public string Name => nameof(DHLExpressAddressValidationProvider);

    public async Task<ValidateAddress> ValidateAddressAsync(ValidateAddress request, CancellationToken cancellationToken = default)
    {
        var watch = ValueStopwatch.StartNew();

        try
        {
            var result = await _dhlExpressApi.ExpApiAddressValidateAsync(
                type: Type2.Delivery,
                countryCode: request.OriginalAddress.CountryCode,
                postalCode: request.OriginalAddress.PostalCode,
                cityName: request.OriginalAddress.City);

            if (result.Warnings == null)
            {
                request.ProposedAddress = new Shipping.Abstractions.Models.Address(
                    request.OriginalAddress?.StreetLine ?? string.Empty,
                    request.OriginalAddress?.StreetLine2 ?? string.Empty,
                    request.OriginalAddress?.City ?? string.Empty,
                    request.OriginalAddress?.StateOrProvince ?? string.Empty,
                    result.Address?.FirstOrDefault()?.PostalCode ?? string.Empty,
                    result.Address?.FirstOrDefault()?.CountryCode ?? string.Empty);
            }
        }
        catch (ApiException<SupermodelIoLogisticsExpressErrorResponse> ex)
        {
            var error = ex?.Result.Detail ?? string.Empty;
            if (ex?.Result?.AdditionalDetails?.Any() ?? false)
            {
                error += string.Join(",", ex.Result.AdditionalDetails);
            }

            _logger.LogError("{name} : {message}", nameof(DHLExpressAddressValidationProvider), error);
            request.InternalErrors.Add(error);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{providerName} failed", nameof(DHLExpressAddressValidationProvider));
            request.InternalErrors.Add(ex?.Message ?? $"{nameof(DHLExpressAddressValidationProvider)} failed");
        }

        _logger.LogDebug("[FedEx][ValidateAddressAsync] completed: {mil}", watch.GetElapsedTime().TotalMilliseconds);
        return request;
    }
}
