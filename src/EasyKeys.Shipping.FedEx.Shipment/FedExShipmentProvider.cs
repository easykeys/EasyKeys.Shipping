using EasyKeys.Shipping.FedEx.Abstractions.Options;
using EasyKeys.Shipping.FedEx.Rates;
using EasyKeys.Shipping.FedEx.Shipment.Extensions;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using ShipClient.v25;

namespace EasyKeys.Shipping.FedEx.Shipment
{
    public class FedExShipmentProvider : IFedExShipmentProvider
    {
        private readonly FedExOptions _options;
        private readonly ILogger<FedExShipmentProvider> _logger;

        public FedExShipmentProvider(
            IOptionsSnapshot<FedExOptions> options,
            ILogger<FedExShipmentProvider> logger)
        {
            _options = options.Value;
            _logger = logger ?? throw new ArgumentException(nameof(logger));
        }

        public Task<ProcessShipmentReply> ProcessShipmentAsync(
            Shipping.Abstractions.Models.Shipment shipment,
            ServiceType serviceType = ServiceType.DEFAULT,
            CancellationToken cancellationToken = default)
        {
            var client = new ShipPortTypeClient(
                ShipPortTypeClient.EndpointConfiguration.ShipServicePort,
                _options.Url);

            try
            {
                var request = CreateShipmentRequest(shipment, serviceType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            return Task.FromResult<ProcessShipmentReply>(new ProcessShipmentReply());
        }

        private ProcessShipmentRequest CreateShipmentRequest(
            Shipping.Abstractions.Models.Shipment shipment,
            ServiceType serviceType)
        {
            var request = new ProcessShipmentRequest
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
                    // TODO: update this if client chooses to use their own
                    AccountNumber = _options.FedExAccountNumber,
                    MeterNumber = _options.FedExMeterNumber
                },
                TransactionDetail = new TransactionDetail
                {
                    CustomerTransactionId = $"Process Shipment Request: {Guid.NewGuid()}"
                },
                Version = new VersionId()
            };
            SetShipmentDetails(
                request,
                shipment,
                serviceType);


            return new ProcessShipmentRequest();
        }

        private void SetShipmentDetails(
            ProcessShipmentRequest reqest,
            Shipping.Abstractions.Models.Shipment shipment,
            ServiceType serviceType)
        {
            reqest.RequestedShipment = new RequestedShipment
            {
                // TODO: Verify that this is correct.
                ShipTimestamp = shipment.Options.ShippingDate ?? DateTime.Now,
                DropoffType = DropoffType.REGULAR_PICKUP,
                ServiceType = serviceType.ToString(),
                PackagingType = shipment.Options.PackagingType,
                PackageCount = shipment.Packages.Count().ToString(),
                TotalWeight = new Weight
                {
                    Value = shipment.Packages.Sum(x => x.Weight),
                    Units = WeightUnits.LB
                },
                RateRequestTypes = GetRateRequestTypes().ToArray(),

                // PackageDetail ?
                // PackageDetailSpecified ?
            };

            SetSender(reqest, shipment);
        }

        private void SetSender(
            ProcessShipmentRequest request,
            Shipping.Abstractions.Models.Shipment shipment)
        {
            request.RequestedShipment.Shipper = new Party
            {
                Contact = new Contact
                {
                    PersonName = "EasyKeys.com Customer Support",
                    CompanyName = "EasyKeys.com",
                    PhoneNumber = "8778395397",

                    // TODO: Verify this is correct
                    EMailAddress = "info@easykeys.com"
                },
                Address = shipment.OriginAddress.GetFedExAddress()
            };
        }

        private void SetRecipient(
            ProcessShipmentRequest request,
            Shipping.Abstractions.Models.Shipment shipment)
        {
            request.RequestedShipment.Recipient = new Party
            {
                // TODO: need a input for contact
                Contact = new Contact
                {
                    PersonName = String.Empty
                }
            };
        }

        private IEnumerable<RateRequestType> GetRateRequestTypes()
        {
            yield return RateRequestType.LIST;
        }
    }
}
