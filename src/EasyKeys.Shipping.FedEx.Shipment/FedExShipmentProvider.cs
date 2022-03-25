using EasyKeys.Shipping.Abstractions.Extensions;
using EasyKeys.Shipping.Abstractions.Models;
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

        public async Task<Label> ProcessShipmentAsync(
            ServiceType serviceType,
            Shipping.Abstractions.Models.Shipment shipment,
            LabelOptions labelOptions,
            CancellationToken cancellationToken = default)
        {
            var client = new ShipPortTypeClient(
                ShipPortTypeClient.EndpointConfiguration.ShipServicePort,
                _options.Url);

            var label = new Label();
            try
            {
                var request = CreateShipmentRequest(
                    serviceType,
                    shipment,
                    labelOptions);

                var shipmentRequest = new processShipmentRequest1(request);

                var response = await client.processShipmentAsync(shipmentRequest);

                var reply = response?.ProcessShipmentReply;

                if ((reply?.HighestSeverity != NotificationSeverityType.ERROR)
                    && (reply?.HighestSeverity != NotificationSeverityType.FAILURE))
                {
                    reply?.CompletedShipmentDetail.CompletedPackageDetails.ToList().
                        ForEach(x =>
                        {
                            label.LabelDetails.Add(
                                new LabelDetails
                                {
                                    ImageType = x.Label.ImageType.ToString(),
                                    Bytes = x.Label.Parts.Select(x => x.Image).ToList()
                                });
                        });
                    return label;
                }

                label.InternalErrors.Add(reply.Notifications[0].Message);
            }
            catch (Exception ex)
            {
                // this does not explain fault exceptions well, must debug handler
                label.InternalErrors.Add($"FedEx provider exception: {ex.Message}");
            }

            return label;
        }

        private ProcessShipmentRequest CreateShipmentRequest(
            ServiceType serviceType,
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
                serviceType,
                shipment,
                labelOptions);

            SetpackageLineItems(
                request,
                shipment,
                labelOptions);

            return request;
        }

        private void SetShipmentDetails(
            ProcessShipmentRequest request,
            ServiceType serviceType,
            Shipping.Abstractions.Models.Shipment shipment,
            LabelOptions labelOptions)
        {
            request.RequestedShipment = new RequestedShipment
            {
                // TODO: Verify that this is correct.
                ShipTimestamp = shipment.Options.ShippingDate ?? DateTime.Now,
                ServiceType = serviceType.ToString(),
                PackagingType = shipment.Options.PackagingType,
                PackageCount = shipment.Packages.Count().ToString(),
                TotalWeight = new Weight
                {
                    Value = shipment.Packages.Sum(x => x.Weight),
                    Units = WeightUnits.LB
                },
                RateRequestTypes = GetRateRequestTypes().ToArray(),
            };
            request.RequestedShipment.DropoffType = shipment.Options.DropOffType switch
            {
                "RegularPickup" => DropoffType.REGULAR_PICKUP,
                "DropBox" => DropoffType.DROP_BOX,
                "BusinessServiceCenter" => DropoffType.BUSINESS_SERVICE_CENTER,
                "RequestCourier" => DropoffType.REQUEST_COURIER,
                "Station" => DropoffType.STATION,
                _ => DropoffType.REGULAR_PICKUP
            };

            SetSender(request, shipment);
            SetRecipient(request, shipment);
            SetPayment(request, shipment, labelOptions);
            SetLabelDetails(request, shipment, labelOptions);
        }

        private void SetSender(
            ProcessShipmentRequest request,
            Shipping.Abstractions.Models.Shipment shipment)
        {
            request.RequestedShipment.Shipper = new Party
            {
                Contact = new ShipClient.v25.Contact()
                {
                    PersonName = shipment.Shipper.FullName,
                    CompanyName = shipment.Shipper.CompanyName,
                    PhoneNumber = shipment.Shipper.PhoneNumber,
                    EMailAddress = shipment.Shipper.Email
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
                // TODO: replace with labelOptions.recipient
                Contact = new ShipClient.v25.Contact()
                {
                    PersonName = shipment.Recipient.FullName,
                    EMailAddress = shipment.Recipient.Email,
                    PhoneNumber = shipment.Recipient.PhoneNumber
                },
                Address = shipment.DestinationAddress.GetFedExAddress()
            };
        }

        private void SetPayment(
            ProcessShipmentRequest request,
            Shipping.Abstractions.Models.Shipment shipment,
            LabelOptions labelOptions)
        {
            switch (labelOptions.PaymentType.ToLower())
            {
                case "sender":
                    request.RequestedShipment.ShippingChargesPayment = new Payment()
                    {
                        // convert to correct type
                        PaymentType = PaymentType.SENDER,
                        Payor = new Payor()
                        {
                            ResponsibleParty = new Party()
                            {
                                AccountNumber = request.ClientDetail.AccountNumber,
                            }
                        }
                    };
                    break;

                case "recipient":
                    request.RequestedShipment.ShippingChargesPayment = new Payment()
                    {
                        PaymentType = PaymentType.RECIPIENT,
                        Payor = new Payor()
                        {
                            // replace with recipient contact information
                            ResponsibleParty = new Party()
                            {
                                Contact = new ShipClient.v25.Contact()
                                {
                                    PersonName = shipment.Recipient.FullName,
                                    EMailAddress = shipment.Recipient.Email,
                                    PhoneNumber = shipment.Recipient.PhoneNumber
                                },
                                AccountNumber = shipment.Recipient.AccountNumber,
                            }
                        }
                    };
                    break;
            }
        }

        private void SetLabelDetails(
            ProcessShipmentRequest request,
            Shipping.Abstractions.Models.Shipment shipment,
            LabelOptions labelOptions)
        {
            request.RequestedShipment.LabelSpecification = new LabelSpecification
            {
                // TODO: replace and update this
                LabelFormatType = labelOptions.LabelFormatType == "COMMON2D" ? LabelFormatType.COMMON2D :
                                  labelOptions.LabelFormatType == "LABEL_DATA_ONLY" ? LabelFormatType.LABEL_DATA_ONLY :
                                  LabelFormatType.COMMON2D,
                ImageType = labelOptions.ImageType == "RTF" ? ShippingDocumentImageType.RTF :
                            labelOptions.ImageType == "EPL2" ? ShippingDocumentImageType.EPL2 :
                            labelOptions.ImageType == "DOC" ? ShippingDocumentImageType.DOC :
                            labelOptions.ImageType == "ZPLII" ? ShippingDocumentImageType.ZPLII :
                            labelOptions.ImageType == "TEXT" ? ShippingDocumentImageType.TEXT :
                            labelOptions.ImageType == "PNG" ? ShippingDocumentImageType.PNG :
                            labelOptions.ImageType == "PDF" ? ShippingDocumentImageType.PDF :
                            ShippingDocumentImageType.PDF,
                ImageTypeSpecified = true,
                PrintedLabelOrigin = new ContactAndAddress()
                {
                    Contact = new ShipClient.v25.Contact()
                    {
                        PersonName = shipment.Shipper.FullName,
                        EMailAddress = shipment.Shipper.Email,
                        PhoneNumber = shipment.Shipper.PhoneNumber
                    },
                    Address = shipment.OriginAddress.GetFedExAddress()
                },
                LabelStockType = labelOptions.LabelSize == "4x6" ? LabelStockType.PAPER_4X6 :
                                 labelOptions.LabelSize == "4x675" ? LabelStockType.PAPER_4X675 :
                                 labelOptions.LabelSize == "4x8" ? LabelStockType.PAPER_4X8 :
                                 labelOptions.LabelSize == "Paper_Letter" ? LabelStockType.PAPER_LETTER :
                                 LabelStockType.PAPER_4X6
            };
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

                // defaults to default for specified service
                if (package.SignatureRequiredOnDelivery)
                {
                    // default to direct signature
                    var signatureOptionDetail = new SignatureOptionDetail { OptionType = SignatureOptionType.SERVICE_DEFAULT };
                    request.RequestedShipment.RequestedPackageLineItems[i].SpecialServicesRequested = new PackageSpecialServicesRequested() { SignatureOptionDetail = signatureOptionDetail };
                }
                else
                {
                    var signatureOptionDetail = new SignatureOptionDetail { OptionType = SignatureOptionType.NO_SIGNATURE_REQUIRED };
                    request.RequestedShipment.RequestedPackageLineItems[i].SpecialServicesRequested = new PackageSpecialServicesRequested() { SignatureOptionDetail = signatureOptionDetail };
                }

                // notification event types
                var eventTypes = new NotificationEventType[] { NotificationEventType.ON_ESTIMATED_DELIVERY, NotificationEventType.ON_SHIPMENT };
                request.RequestedShipment.SpecialServicesRequested = new ShipmentSpecialServicesRequested
                {
                    SpecialServiceTypes = new string[] { "EVENT_NOTIFICATION" },
                    EventNotificationDetail = new ShipmentEventNotificationDetail
                    {
                        // notification event message
                        //PersonalMessage = "pacakge is shipped",
                        EventNotifications = new ShipmentEventNotificationSpecification[]
                        {
                            new ShipmentEventNotificationSpecification
                            {
                                Events = eventTypes,
                                NotificationDetail = new NotificationDetail
                                {
                                    NotificationType = NotificationType.EMAIL,
                                    EmailDetail = new EMailDetail()
                                    {
                                        EmailAddress = shipment.Recipient.Email,
                                        Name = shipment.Recipient.FullName,
                                    },
                                    Localization = new Localization
                                    {
                                        LanguageCode = "EN"
                                    }
                                },
                                FormatSpecification = new ShipmentNotificationFormatSpecification
                                {
                                    Type = NotificationFormatType.HTML,
                                },
                                Role = ShipmentNotificationRoleType.RECIPIENT,
                                RoleSpecified = true
                            }
                        },
                    }
                };

                request.RequestedShipment.CustomsClearanceDetail = new CustomsClearanceDetail
                {
                    CommercialInvoice = new CommercialInvoice
                    {
                        CustomerReferences = new CustomerReference[]
                        {
                            new CustomerReference
                            {
                                CustomerReferenceType = CustomerReferenceType.CUSTOMER_REFERENCE,
                                Value = request.TransactionDetail.CustomerTransactionId
                            }
                        }
                    }
                };

                if (labelOptions.CollectOnDelivery.Activated)
                {
                    request.RequestedShipment.SpecialServicesRequested = new ShipmentSpecialServicesRequested
                    {
                        // TODO: update cod detail information
                        SpecialServiceTypes = new string[] { "COD" },
                        CodDetail = new CodDetail()
                        {
                            CodCollectionAmount = new Money()
                            {
                                Amount = labelOptions.CollectOnDelivery.Amount,
                                Currency = labelOptions.CollectOnDelivery.Currency
                            },
                            CollectionType = labelOptions.CollectOnDelivery.CollectionType == "GUARANTEED_FUNDS" ? CodCollectionType.GUARANTEED_FUNDS :
                                             labelOptions.CollectOnDelivery.CollectionType == "CASH" ? CodCollectionType.CASH :
                                             labelOptions.CollectOnDelivery.CollectionType == "ANY" ? CodCollectionType.ANY :
                                             labelOptions.CollectOnDelivery.CollectionType == "COMPANY_CHECK" ? CodCollectionType.COMPANY_CHECK :
                                             CodCollectionType.GUARANTEED_FUNDS,
                        }
                    };
                }

                i++;
            }
        }

        private IEnumerable<RateRequestType> GetRateRequestTypes()
        {
            yield return RateRequestType.LIST;
        }
    }
}
