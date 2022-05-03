using AddressValidationClient.v4;

using EasyKeys.Shipping.Abstractions;
using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.FedEx.Abstractions.Options;
using EasyKeys.Shipping.FedEx.Abstractions.Services;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using v4 = AddressValidationClient.v4;

namespace EasyKeys.Shipping.FedEx.AddressValidation;

public class FedExAddressValidationProvider : IFedExAddressValidationProvider
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

    public async Task<ValidateAddress> ValidateAddressAsync(ValidateAddress request, CancellationToken cancellationToken = default)
    {
        try
        {
            // don't waste resources
            if (!AddressAttributes.SupportedCountries.ContainsKey(request.OriginalAddress.CountryCode))
            {
                return request;
            }

            var client = _addressValidationClient;
            var wrap = CreateRequest(request);

            var serviceRequest = new v4.addressValidationRequest1(wrap);

            var reply = await client.addressValidationAsync(serviceRequest);
            if (reply.AddressValidationReply.HighestSeverity == v4.NotificationSeverityType.SUCCESS
                || reply.AddressValidationReply.HighestSeverity == v4.NotificationSeverityType.NOTE
                || reply.AddressValidationReply.HighestSeverity == v4.NotificationSeverityType.WARNING)
            {
                var result = reply.AddressValidationReply;
                var addressResults = result.AddressResults[0];
                var effectiveAddress = addressResults.EffectiveAddress;
                var parsedAddress = addressResults.ParsedAddressPartsDetail;
                var lines = effectiveAddress?.StreetLines ?? new string[1] { string.Empty };

                request.ProposedAddress = new Shipping.Abstractions.Models.Address(
                    lines[0],
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
                    v4.FedExAddressClassificationType.MIXED => false,
                    v4.FedExAddressClassificationType.BUSINESS => false,
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
            }
        }
        catch (Exception ex)
        {
            var exMsg = "FedExValidation client failed";
            _logger.LogError(ex, exMsg);
            request.InternalErrors.Add(ex?.Message ?? exMsg);
        }

        return request;
    }

    private v4.AddressValidationRequest CreateRequest(ValidateAddress request)
    {
        var address = request.ProposedAddress;

        return new v4.AddressValidationRequest
        {
            WebAuthenticationDetail = new v4.WebAuthenticationDetail
            {
                UserCredential = new v4.WebAuthenticationCredential
                {
                    Key = _options.FedExKey,
                    Password = _options.FedExPassword
                }
            },

            ClientDetail = new v4.ClientDetail
            {
                AccountNumber = _options.FedExAccountNumber,
                MeterNumber = _options.FedExMeterNumber
            },

            TransactionDetail = new v4.TransactionDetail
            {
                CustomerTransactionId = nameof(FedExAddressValidationProvider)
            },

            Version = new v4.VersionId(),
            AddressesToValidate = new v4.AddressToValidate[1]
            {
                    new v4.AddressToValidate
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
                    }
            },

            InEffectAsOfTimestamp = DateTime.Now,
            InEffectAsOfTimestampSpecified = true
        };
    }
}
