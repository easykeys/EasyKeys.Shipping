using EasyKeys.Shipping.Abstractions.Extensions;
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

        public async Task<ProcessShipmentReply> ProcessShipmentAsync(
            Shipping.Abstractions.Models.Shipment shipment,
            ServiceType serviceType = ServiceType.DEFAULT,
            bool CODShipment = false,
            CancellationToken cancellationToken = default)
        {
            var client = new ShipPortTypeClient(
                ShipPortTypeClient.EndpointConfiguration.ShipServicePort,
                _options.Url);

            try
            {
                var request = CreateShipmentRequest(
                    shipment,
                    serviceType,
                    CODShipment);

                LogXML(request, typeof(ProcessShipmentRequest));
                var shipmentRequest = new processShipmentRequest1(request);
                LogXML(shipmentRequest, typeof(processShipmentRequest1));
                var reply = await client.processShipmentAsync(shipmentRequest);

                if (reply.ProcessShipmentReply != null)
                {
                    _logger.LogCritical("it worked..");
                    return reply.ProcessShipmentReply;
                }
            }
            catch (Exception ex)
            {

                _logger.LogError(ex.Data.ToString());
            }

            return new ProcessShipmentReply();
        }

        private ProcessShipmentRequest CreateShipmentRequest(
            Shipping.Abstractions.Models.Shipment shipment,
            ServiceType serviceType,
            bool codShipment)
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
                    CustomerTransactionId = $"Process Shipment Request: {Guid.NewGuid().ToString()}"
                },
                Version = new VersionId()
            };
            SetShipmentDetails(
                request,
                shipment,
                serviceType);

            SetpackageLineItems(
                request,
                shipment,
                codShipment);

            _logger.LogCritical("Create Shipment Request Complete");
            return request;
        }

        private void LogXML(Object obj, Type type)
        {
            var serializer =
                new System.Xml.Serialization.XmlSerializer(type);
            TextWriter writer = new StreamWriter("..\\access.log", true);
            writer.WriteLine("-------------" + DateTime.Now.ToString() + "-------------");
            serializer.Serialize(writer, obj);
            writer.WriteLine();
            writer.WriteLine("____________________________________________________");
            writer.WriteLine();
            writer.Close();
        }

        private void SetShipmentDetails(
            ProcessShipmentRequest request,
            Shipping.Abstractions.Models.Shipment shipment,
            ServiceType serviceType)
        {
            request.RequestedShipment = new RequestedShipment
            {
                // TODO: Verify that this is correct.
                ShipTimestamp = shipment.Options.ShippingDate ?? DateTime.Now,
                DropoffType = DropoffType.REGULAR_PICKUP,
                ServiceType = ServiceType.FEDEX_2_DAY.ToString(),
                PackagingType = FedExPackageType.YOUR_PACKAGING.ToString(),
                PackageCount = shipment.Packages.Count().ToString(),
                TotalWeight = new Weight
                {
                    Value = 500.00M,
                    Units = WeightUnits.LB
                },
                RateRequestTypes = GetRateRequestTypes().ToArray(),

                // PackageDetail ?
                // PackageDetailSpecified ?
            };

            SetSender(request, shipment);
            SetRecipient(request, shipment);
            SetPayment(request);
            SetLabelDetails(request);
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
            _logger.LogCritical("Set Sender Complete");
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
                    PersonName = "brandon",
                    CompanyName = "mcm",
                    PhoneNumber = "999-888-7777",
                    EMailAddress = "bmoff@gmail.com"
                },
                Address = shipment.DestinationAddress.GetFedExAddress()
            };

            // TODO: Set up Special Services
            _logger.LogCritical("Set Recipient Complete");
        }

        private void SetPayment(ProcessShipmentRequest request)
        {
            // TODO: logic for account number to switch
            request.RequestedShipment.ShippingChargesPayment = new Payment
            {
                PaymentType = PaymentType.SENDER,
                Payor = new Payor()
                {
                    ResponsibleParty = new Party()
                    {
                        AccountNumber = request.ClientDetail.AccountNumber,
                    }
                }
            };
            _logger.LogCritical("Set Payment Complete");
        }

        private void SetLabelDetails(ProcessShipmentRequest request)
        {
            request.RequestedShipment.LabelSpecification = new LabelSpecification
            {
                LabelFormatType = LabelFormatType.COMMON2D,
                ImageType = ShippingDocumentImageType.PDF,
                ImageTypeSpecified = true,
                PrintedLabelOrigin = new ContactAndAddress
                {
                    Contact = new Contact
                    {
                        CompanyName = "Fulfillment Center",
                        PhoneNumber = "888.888.8888"
                    },
                    // verify this is correct
                    Address = request.RequestedShipment.Shipper.Address
                }
            };
            _logger.LogCritical("Set Label Details Complete");
        }

        private void SetpackageLineItems(
            ProcessShipmentRequest request,
            Shipping.Abstractions.Models.Shipment shipment,
            bool codShipment)
        {
            request.RequestedShipment.RequestedPackageLineItems = new RequestedPackageLineItem[shipment.Packages.Count()];
            var i = 0;
            foreach (var package in shipment.Packages)
            {
                request.RequestedShipment.RequestedPackageLineItems[i] = new RequestedPackageLineItem()
                {
                    SequenceNumber = (i + 1).ToString(),
                    GroupPackageCount = "1",

                    Weight = new Weight()
                    {
                        Units = WeightUnits.LB,
                        Value = package.RoundedWeight,
                    },

                    Dimensions = new Dimensions()
                    {
                        Length = package.Dimensions.RoundedLength.ToString(),
                        Width = package.Dimensions.RoundedWidth.ToString(),
                        Height = package.Dimensions.RoundedHeight.ToString(),
                        Units = LinearUnits.IN
                    }
                };

                request.RequestedShipment.RequestedPackageLineItems[i].InsuredValue = new Money
                {
                    Amount = package.InsuredValue,
                    Currency = shipment.Options.GetCurrencyCode()
                };

                if (package.SignatureRequiredOnDelivery)
                {
                    var signatureOptionDetail = new SignatureOptionDetail { OptionType = SignatureOptionType.INDIRECT };
                    request.RequestedShipment.RequestedPackageLineItems[i].SpecialServicesRequested = new PackageSpecialServicesRequested() { SignatureOptionDetail = signatureOptionDetail };
                }

                // TODO: set up customer references

                if (codShipment)
                {
                    request.RequestedShipment.SpecialServicesRequested = new ShipmentSpecialServicesRequested
                    {
                        // TODO: Special Service Types ?
                        CodDetail = new CodDetail
                        {
                            CollectionType = CodCollectionType.GUARANTEED_FUNDS,
                            CodCollectionAmount = new Money()
                            {
                                Amount = 250,
                                Currency = "USD"
                            }
                        }
                    };
                }

                i++;
            }

            _logger.LogCritical("Set Package Line Items Details Complete");
        }

        private IEnumerable<RateRequestType> GetRateRequestTypes()
        {
            yield return RateRequestType.LIST;
        }
    }
}
