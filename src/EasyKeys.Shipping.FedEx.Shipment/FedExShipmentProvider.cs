using EasyKeys.Shipping.Abstractions.Extensions;
using EasyKeys.Shipping.FedEx.Abstractions.Options;
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

        public async Task<Label> ProcessShipmentAsync(
            Shipping.Abstractions.Models.Shipment shipment,
            LabelOptions labelOptions,
            CancellationToken cancellationToken = default)
        {
            var client = new ShipPortTypeClient(
                ShipPortTypeClient.EndpointConfiguration.ShipServicePort,
                _options.Url);

            try
            {
                var request = CreateShipmentRequest(
                    shipment,
                    labelOptions);

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
                        labelOptions,
                        reply.CompletedShipmentDetail,
                        reply.CompletedShipmentDetail
                            .CompletedPackageDetails
                                .FirstOrDefault());
                    return new Label()
                    {
                        ShippingDocumentImageType = reply.CompletedShipmentDetail
                                    .CompletedPackageDetails
                                    .FirstOrDefault()
                                    .Label.ImageType,

                        Bytes = reply.CompletedShipmentDetail
                                    .CompletedPackageDetails
                                    .FirstOrDefault()
                                    .Label.Parts[0].Image
                    };
                }

                _logger.LogInformation(reply.Notifications[0].Message);
            }
            catch (Exception ex)
            {
                // this does not explain fault exceptions well, must debug handler
                _logger.LogError(ex.Message);
            }

            return new Label();
        }

        private ProcessShipmentRequest CreateShipmentRequest(
            Shipping.Abstractions.Models.Shipment shipment,
            LabelOptions labelOptions)
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
                labelOptions);

            SetpackageLineItems(
                request,
                shipment,
                labelOptions);

            _logger.LogCritical("Create Shipment Request Complete");
            return request;
        }

        private void SetShipmentDetails(
            ProcessShipmentRequest request,
            Shipping.Abstractions.Models.Shipment shipment,
            LabelOptions labelOptions)
        {
            request.RequestedShipment = new RequestedShipment
            {
                // TODO: Verify that this is correct.
                ShipTimestamp = shipment.Options.ShippingDate ?? DateTime.Now,
                DropoffType = labelOptions.DropoffType,

                // does not work with default
                ServiceType = labelOptions.ServiceType.ToString(),
                PackagingType = labelOptions.PackageType.ToString(),
                PackageCount = shipment.Packages.Count().ToString(),
                TotalWeight = new Weight
                {
                    Value = shipment.Packages.Sum(x => x.Weight),
                    Units = labelOptions.Units
                },
                RateRequestTypes = GetRateRequestTypes().ToArray(),
            };

            SetSender(request, shipment, labelOptions);
            SetRecipient(request, shipment, labelOptions);
            SetPayment(request, labelOptions);
            SetLabelDetails(request, labelOptions);
        }

        private void SetSender(
            ProcessShipmentRequest request,
            Shipping.Abstractions.Models.Shipment shipment,
            LabelOptions labelOptions)
        {
            request.RequestedShipment.Shipper = new Party
            {
                Contact = labelOptions.ShipperContact,
                Address = shipment.OriginAddress.GetFedExAddress()
            };
            _logger.LogCritical("Set Sender Complete");
        }

        private void SetRecipient(
            ProcessShipmentRequest request,
            Shipping.Abstractions.Models.Shipment shipment,
            LabelOptions labelOptions)
        {
            request.RequestedShipment.Recipient = new Party
            {
                // TODO: need a input for contact
                Contact = labelOptions.RecipientContact,
                Address = shipment.DestinationAddress.GetFedExAddress()
            };

            // TODO: Set up Special Services - email
            _logger.LogCritical("Set Recipient Complete");
        }

        private void SetPayment(
            ProcessShipmentRequest request,
            LabelOptions labelOptions)
        {
            // TODO: logic to receive user account number
            switch (labelOptions.PaymentType)
            {
                case PaymentType.SENDER:
                    request.RequestedShipment.ShippingChargesPayment = new Payment()
                    {
                        PaymentType = labelOptions.PaymentType,
                        Payor = new Payor()
                        {
                            ResponsibleParty = new Party()
                            {
                                AccountNumber = request.ClientDetail.AccountNumber,
                            }
                        }
                    };
                    break;

                default:
                    request.RequestedShipment.ShippingChargesPayment = new Payment()
                    {
                        PaymentType = labelOptions.PaymentType,
                        Payor = new Payor()
                        {
                            ResponsibleParty = labelOptions.ResponsibleParty
                        }
                    };
                    break;
            }

            _logger.LogCritical("Set Payment Complete");
        }

        private void SetLabelDetails(
            ProcessShipmentRequest request,
            LabelOptions labelOptions)
        {
            request.RequestedShipment.LabelSpecification = new LabelSpecification
            {
                LabelFormatType = labelOptions.LabelFormatType,
                ImageType = labelOptions.ShippingDocumentImageType,
                ImageTypeSpecified = true,
                PrintedLabelOrigin = labelOptions.FulfillmentContactAndAddress
            };
            _logger.LogCritical("Set Label Details Complete");
        }

        private void SetpackageLineItems(
            ProcessShipmentRequest request,
            Shipping.Abstractions.Models.Shipment shipment,
            LabelOptions labelOptions)
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
                        Units = labelOptions.Units,
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
                    var signatureOptionDetail = new SignatureOptionDetail { OptionType = labelOptions.SignatureOptionType };
                    request.RequestedShipment.RequestedPackageLineItems[i].SpecialServicesRequested = new PackageSpecialServicesRequested() { SignatureOptionDetail = signatureOptionDetail };
                }

                // TODO: set up customer references
                if (labelOptions.IsCodShipment)
                {
                    request.RequestedShipment.SpecialServicesRequested = new ShipmentSpecialServicesRequested
                    {
                        // TODO: Special Service Types ?
                        CodDetail = labelOptions.CodDetail
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

        private void ShowShipmentLabels(LabelOptions labelOptions, CompletedShipmentDetail completedShipmentDetail, CompletedPackageDetail packageDetail)
        {
            if (packageDetail.Label.Parts[0].Image != null)
            {
                // Save outbound shipping label
                var labelPath = "..\\EasyKeys.Shipping.FedEx.Shipment\\Labels\\";
                var labelFileName = string.Empty;
                switch (labelOptions.ShippingDocumentImageType)
                {
                    case ShippingDocumentImageType.PDF:
                        labelFileName = labelPath + packageDetail.TrackingIds[0].TrackingNumber + ".pdf";
                        break;
                    case ShippingDocumentImageType.PNG:
                        labelFileName = labelPath + packageDetail.TrackingIds[0].TrackingNumber + ".png";
                        break;
                    case ShippingDocumentImageType.EPL2:
                        labelFileName = labelPath + packageDetail.TrackingIds[0].TrackingNumber + ".epl2";
                        break;
                }

                SaveLabel(labelFileName, packageDetail.Label.Parts[0].Image);
                if (labelOptions.IsCodShipment)
                {
                    // Save COD Return label
                    labelFileName = labelPath + completedShipmentDetail.AssociatedShipments[0].TrackingId.TrackingNumber + "CR" + ".png";
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
