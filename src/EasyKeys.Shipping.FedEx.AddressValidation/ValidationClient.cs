using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

using AddressValidationClient.v4;

using EasyKeys.Shipping.FedEx.Abstractions.Options;
using EasyKeys.Shipping.FedEx.AddressValidation.Models;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EasyKeys.Shipping.FedEx.AddressValidation
{
    public class ValidationClient : IValidationClient
    {
        private readonly ILogger<ValidationClient> _logger;
        private FedExOptions _options;

        public ValidationClient(
            IOptionsMonitor<FedExOptions> optionsMonitor,
            ILogger<ValidationClient> logger)
        {
            _options = optionsMonitor.CurrentValue;
            optionsMonitor.OnChange(n => _options = n);

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public ValidationResponse InputValidation(ValidationRequest request)
        {
            var result = new ValidationResponse
            {
                OriginalAddress = new PostalAddress
                {
                    Address = request.Address.Address,
                    Address2 = request.Address.Address2,
                    City = request.Address.City,
                    StateOrProvince = request.Address.StateOrProvince,
                    PostalCode = request.Address.PostalCode,
                    CountryCode = request.Address.CountryCode,
                },
                IsValid = false,
                IsConfirmedBusiness = !request.Address.IsResidential
            };

            var errorMessage = string.Empty;

            if (string.Equals(request.Address.Address, request.Address.Address2, StringComparison.OrdinalIgnoreCase))
            {
                result.ValidationErrors = " Address Line 1 can't be the same in Address Line 2 ";
                return result;
            }

            var attention = new List<string>
            {
                "C/O",
                "ATTN",
                "ATTENTION"
            };

            foreach (var item in attention)
            {
                if (result.OriginalAddress.Address.ToUpper().Contains(item) || result.OriginalAddress.Address2.ToUpper().Contains(item))
                {
                    errorMessage += $" {item} can't be in Address Line 1 or Address Line 2. Please use Delivery Instructions ";
                }
            }

            if (!string.IsNullOrEmpty(errorMessage))
            {
                result.ValidationErrors = errorMessage;
                return result;
            }

            result.IsValid = true;

            return result;
        }

        public async Task<ValidationResponse> ValidateAddressAsync(ValidationRequest request)
        {
            var response = new ValidationResponse
            {
                IsValid = false,
                OriginalAddress = request.Address
            };

            try
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                var client = new AddressValidationPortTypeClient(
                        AddressValidationPortTypeClient.EndpointConfiguration.AddressValidationServicePort,
                        _options.Url);

                var wrap = CreateAddressValidationRequest(request);

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

                    response.ProposedAddress = new PostalAddress
                    {
                        Address = effectiveAddress.StreetLines[0],
                        City = effectiveAddress.City,
                        StateOrProvince = effectiveAddress.StateOrProvinceCode,
                        PostalCode = effectiveAddress.PostalCode,
                        CountryCode = effectiveAddress.CountryCode,
                        IsResidential = effectiveAddress.Residential
                    };

                    if (effectiveAddress.StreetLines.Length == 2)
                    {
                        response.ProposedAddress.Address2 = effectiveAddress.StreetLines[1];
                    }

                    response.IsConfirmedBusiness = !effectiveAddress.Residential;
                    response.IsValid = true;

                    switch (addressResults.State)
                    {
                        case OperationalAddressStateType.RAW:
                            response.Score = 0;
                            break;
                        case OperationalAddressStateType.NORMALIZED:
                            response.Score = 60;
                            break;
                        case OperationalAddressStateType.STANDARDIZED:
                            response.Score = 90;
                            break;
                    }
                }
                else
                {
                    var exMsg = string.Empty;
                    foreach (var notification in reply.AddressValidationReply.Notifications)
                    {
                        exMsg += notification.Message;
                    }

                    response.ValidationErrors = exMsg;
                }
            }
            catch (Exception ex)
            {
                var exMsg = "FedExValidation client failed";
                _logger.LogError(ex, exMsg);

                response.ValidationErrors = ex?.Message ?? exMsg;
            }

            return response;
        }

        private AddressValidationRequest CreateAddressValidationRequest(ValidationRequest addressRequest)
        {
            // Build the AddressValidationRequest
            var request = new AddressValidationRequest();

            request.WebAuthenticationDetail = new WebAuthenticationDetail
            {
                UserCredential = new WebAuthenticationCredential
                {
                    Key = _options.FedExKey,
                    Password = _options.FedExPassword
                },

                ParentCredential = new WebAuthenticationCredential
                {
                    Key = _options.FedExKey,
                    Password = _options.FedExPassword
                }
            };

            request.ClientDetail = new ClientDetail
            {
                AccountNumber = _options.FedExAccountNumber,
                MeterNumber = _options.FedExMeterNumber
            };

            request.TransactionDetail = new TransactionDetail
            {
                CustomerTransactionId = nameof(ValidationClient)
            };

            request.Version = new VersionId(); // Creates the Version element with all child elements populated

            request.InEffectAsOfTimestamp = DateTime.Now;
            request.InEffectAsOfTimestampSpecified = true;

            SetAddress(request, addressRequest);

            return request;
        }

        private void SetAddress(AddressValidationRequest serviceRequest, ValidationRequest addressRequest)
        {
            serviceRequest.AddressesToValidate = new AddressToValidate[1];

            serviceRequest.AddressesToValidate[0] = new AddressToValidate
            {
                ClientReferenceId = "ClientReferenceId1",
                Address = new Address
                {
                    StreetLines = new string[2] { addressRequest.Address.Address, addressRequest.Address.Address2 },
                    PostalCode = addressRequest.Address.PostalCode,
                    City = addressRequest.Address.City,
                    StateOrProvinceCode = addressRequest.Address.StateOrProvince,
                    CountryCode = addressRequest.Address.CountryCode
                }
            };
        }
    }
}
