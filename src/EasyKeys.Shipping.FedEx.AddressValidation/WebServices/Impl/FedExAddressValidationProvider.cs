using System.Diagnostics;

using AddressValidationClient.v4;

using EasyKeys.Shipping.Abstractions;
using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.FedEx.Abstractions.Options;
using EasyKeys.Shipping.FedEx.Abstractions.Services;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using v4 = AddressValidationClient.v4;

namespace EasyKeys.Shipping.FedEx.AddressValidation.WebServices.Impl;

/// <summary>
/// Caution: FedEx Web Services Tracking, Address Validation, and Validate Postal Codes WSDLS will be disabled on August 31, 2024. The SOAP based FedEx Web Services is in development containment and has been replaced with FedEx RESTful APIs. To learn more and upgrade your integration from Web Services to FedEx APIs, please visit the FedEx Developer Portal.
/// </summary>
public class FedExAddressValidationProvider : IFedExAddressValidationProvider, IAddressValidationProvider
{
    private readonly ILogger<FedExAddressValidationProvider> _logger;
    private readonly AddressValidationPortType _addressValidationClient;
    private FedExOptions _options;

    public FedExAddressValidationProvider(
        IOptionsMonitor<FedExOptions> optionsMonitor,
        IFedExClientService fedExClient,
        ILogger<FedExAddressValidationProvider> logger)
    {
        _options = optionsMonitor.CurrentValue;

        _addressValidationClient = fedExClient.CreateAddressValidationClient();

        optionsMonitor.OnChange(n => _options = n);

        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public string Name => nameof(FedExAddressValidationProvider);

    public async Task<ValidateAddress> ValidateAddressAsync(ValidateAddress request, CancellationToken cancellationToken = default)
    {
        var watch = ValueStopwatch.StartNew();

        try
        {
            var client = _addressValidationClient;
            var wrap = CreateRequest(request);

            var serviceRequest = new addressValidationRequest1(wrap);

            var reply = await client.addressValidationAsync(serviceRequest);

            if (reply.AddressValidationReply.HighestSeverity == NotificationSeverityType.SUCCESS
                || reply.AddressValidationReply.HighestSeverity == NotificationSeverityType.NOTE
                || reply.AddressValidationReply.HighestSeverity == NotificationSeverityType.WARNING)
            {
                var result = reply.AddressValidationReply;
                var addressResults = result.AddressResults[0];
                var effectiveAddress = addressResults.EffectiveAddress;
                var parsedAddress = addressResults.ParsedAddressPartsDetail;

                var lines = effectiveAddress?.StreetLines ?? new string[1] { string.Empty };
                var address1 = lines[0];
                var address2 = lines.Length > 1 ? lines[1] : string.Empty;

                request.ProposedAddress = new Shipping.Abstractions.Models.Address(
                    address1,
                    address2,
                    effectiveAddress?.City ?? string.Empty,
                    effectiveAddress?.StateOrProvinceCode ?? string.Empty,
                    effectiveAddress?.PostalCode ?? string.Empty,
                    effectiveAddress?.CountryCode ?? string.Empty,
                    effectiveAddress?.Residential ?? true);

                if (lines.Length == 2)
                {
                    request.ProposedAddress.StreetLine2 = lines[1];
                }

                request.ProposedAddress.IsResidential = addressResults.Classification switch
                {
                    FedExAddressClassificationType.MIXED => false,
                    FedExAddressClassificationType.BUSINESS => false,
                    _ => true,
                };

                if (!request.ValidationBag.ContainsKey("Classification"))
                {
                    request.ValidationBag.Add("Classification", addressResults.Classification.ToString());
                }

                if (!request.ValidationBag.ContainsKey("State"))
                {
                    request.ValidationBag.Add("State", addressResults.State.ToString());
                }

                foreach (var a in addressResults.Attributes)
                {
                    if (!request.ValidationBag.ContainsKey(a.Name))
                    {
                        request.ValidationBag.Add(a.Name, a.Value);
                    }
                }
            }
            else
            {
                var exMsg = string.Empty;
                foreach (var notification in reply.AddressValidationReply.Notifications)
                {
                    exMsg += notification.Message;
                    request.Errors.Add(new Error
                    {
                        Source = notification.Source,
                        Number = notification.Code,
                        Description = notification.Message
                    });
                }

                _logger.LogError("{providerName} failed: {errors} ", nameof(FedExAddressValidationProvider), request.Errors.Select(x => x.Description).Flatten(","));
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

    private AddressValidationRequest CreateRequest(ValidateAddress request)
    {
        var address = request.OriginalAddress;

        var addressToValidate = new AddressToValidate
        {
            ClientReferenceId = request.Id,
            Address = new v4.Address
            {
                StreetLines = address?.GetStreetLines(),
                PostalCode = address?.PostalCode,
                City = address?.City,
                StateOrProvinceCode = address?.StateOrProvince,
                CountryCode = address?.CountryCode
            }
        };

        return new AddressValidationRequest
        {
            WebAuthenticationDetail = new WebAuthenticationDetail
            {
                UserCredential = new WebAuthenticationCredential
                {
                    Key = _options.FedExKey,
                    Password = _options.FedExPassword
                }
            },

            ClientDetail = new ClientDetail
            {
                AccountNumber = _options.FedExAccountNumber,
                MeterNumber = _options.FedExMeterNumber
            },

            TransactionDetail = new TransactionDetail
            {
                CustomerTransactionId = nameof(FedExAddressValidationProvider)
            },

            Version = new VersionId(),
            AddressesToValidate = new AddressToValidate[1]
            {
                   addressToValidate
            },

            InEffectAsOfTimestamp = DateTime.Now,
            InEffectAsOfTimestampSpecified = true
        };
    }
}
