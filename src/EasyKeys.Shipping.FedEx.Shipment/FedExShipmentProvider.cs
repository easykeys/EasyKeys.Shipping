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
            bool isCodShipment = false,
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
                    isCodShipment);

                // for testing purposes
                LogXML(request, typeof(ProcessShipmentRequest));
                var shipmentRequest = new processShipmentRequest1(request);
                var response = await client.processShipmentAsync(shipmentRequest);
                var reply = response?.ProcessShipmentReply;

                if ((reply?.HighestSeverity != NotificationSeverityType.ERROR)
                    && (reply?.HighestSeverity != NotificationSeverityType.FAILURE))
                {
                    // for testing purposes
                    ShowShipmentLabels(
                        isCodShipment,
                        reply.CompletedShipmentDetail,
                        reply.CompletedShipmentDetail
                            .CompletedPackageDetails
                                .FirstOrDefault());
                    return reply;
                }
            }
            catch (Exception ex)
            {
                // this does not explain fault exceptions well, must debug handler
                _logger.LogError(ex.Message);
            }

            return new ProcessShipmentReply();
        }

        private ProcessShipmentRequest CreateShipmentRequest(
            Shipping.Abstractions.Models.Shipment shipment,
            ServiceType serviceType,
            bool isCodShipment)
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
                isCodShipment,
                serviceType);

            SetpackageLineItems(
                request,
                shipment,
                isCodShipment);

            _logger.LogCritical("Create Shipment Request Complete");
            return request;
        }

        private void SetShipmentDetails(
            ProcessShipmentRequest request,
            Shipping.Abstractions.Models.Shipment shipment,
            bool isCodShipment,
            ServiceType serviceType)
        {
            request.RequestedShipment = new RequestedShipment
            {
                // TODO: Verify that this is correct.
                ShipTimestamp = shipment.Options.ShippingDate ?? DateTime.Now,
                DropoffType = DropoffType.REGULAR_PICKUP,

                // does not work with default
                ServiceType = ServiceType.FEDEX_2_DAY.ToString(),
                PackagingType = FedExPackageType.YOUR_PACKAGING.ToString(),
                PackageCount = shipment.Packages.Count().ToString(),
                TotalWeight = new Weight
                {
                    Value = shipment.Packages.Sum(x => x.Weight),
                    Units = WeightUnits.LB
                },
                RateRequestTypes = GetRateRequestTypes().ToArray(),
            };

            SetSender(request, shipment);
            SetRecipient(request, shipment);
            SetPayment(request, isCodShipment);
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

            // TODO: Set up Special Services - email
            _logger.LogCritical("Set Recipient Complete");
        }

        private void SetPayment(ProcessShipmentRequest request, bool isCodShipment)
        {
            // TODO: logic to receive user account number
            if (isCodShipment)
            {
                request.RequestedShipment.ShippingChargesPayment = new Payment
                {
                    PaymentType = PaymentType.RECIPIENT,
                    Payor = new Payor()
                    {
                        ResponsibleParty = new Party()
                        {
                            AccountNumber = "234564646346345" // input for recipent account number
                        }
                    }
                };
                return;
            }

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
            bool isCodShipment)
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
                if (isCodShipment)
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

        private void ShowShipmentLabels(bool isisCodShipment, CompletedShipmentDetail completedShipmentDetail, CompletedPackageDetail packageDetail)
        {
            if (packageDetail.Label.Parts[0].Image != null)
            {
                // Save outbound shipping label
                var labelPath = "..\\EasyKeys.Shipping.FedEx.Shipment\\Labels\\";

                var labelFileName = labelPath + packageDetail.TrackingIds[0].TrackingNumber + ".pdf";
                SaveLabel(labelFileName, packageDetail.Label.Parts[0].Image);
                if (isisCodShipment)
                {
                    // Save COD Return label
                    labelFileName = labelPath + completedShipmentDetail.AssociatedShipments[0].TrackingId.TrackingNumber + "CR" + ".pdf";
                    SaveLabel(labelFileName, completedShipmentDetail.AssociatedShipments[0].Label.Parts[0].Image);
                }
            }
        }

        private void SaveLabel(string labelFileName, byte[] labelBuffer)
        {
            // Save label buffer to file
            var labelFile = new FileStream(labelFileName, FileMode.Create);
            labelFile.Write(labelBuffer, 0, labelBuffer.Length);
            labelFile.Close();
            _logger.LogInformation("Label saved to {location}", labelFile);

            // Display label in Acrobat
            DisplayLabel(labelFileName);
        }

        private void DisplayLabel(string labelFileName)
        {
            var info = new System.Diagnostics.ProcessStartInfo(labelFileName);
            info.UseShellExecute = true;
            info.Verb = "open";
            System.Diagnostics.Process.Start(info);
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
    }
}
