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
            var masterTrackingId = new TrackingId();
            try
            {
                for (var i = 0; i < shipment.Packages.Count(); i++)
                {
                    var request = CreateShipmentRequest(
                            serviceType,
                            shipment,
                            labelOptions,
                            i);

                    if (i > 0)
                    {
                        // child package of main package, create multiple package service
                        request.RequestedShipment.MasterTrackingId = masterTrackingId;
                    }

                    var shipmentRequest = new processShipmentRequest1(request);

                    var response = await client.processShipmentAsync(shipmentRequest);

                    var reply = response?.ProcessShipmentReply;

                    if ((reply?.HighestSeverity != NotificationSeverityType.ERROR)
                             && (reply?.HighestSeverity != NotificationSeverityType.FAILURE))
                    {
                        var charges = new Charges();
                        reply?.CompletedShipmentDetail.CompletedPackageDetails.ToList().
                            ForEach(x =>
                            {
                                charges.SurCharges = new Dictionary<string, decimal>();

                                charges.BaseCharge = x.PackageRating.PackageRateDetails[0].BaseCharge.Amount;

                                charges.NetCharge = x.PackageRating.PackageRateDetails[0].NetCharge.Amount;

                                x.PackageRating.PackageRateDetails[0].Surcharges.ToList().
                                    ForEach(x => charges.SurCharges[x.Description] = x.Amount.Amount);

                                charges.TotalSurCharges = x.PackageRating.PackageRateDetails[0].TotalSurcharges.Amount;

                                label.LabelDetails.Add(
                                    new LabelDetails
                                    {
                                        Charges = charges,

                                        TrackingId = x.TrackingIds[0].TrackingNumber,

                                        ImageType = x.Label.ImageType.ToString(),

                                        Bytes = x.Label.Parts.Select(x => x.Image).ToList()
                                    });
                            });

                        label.MasterTrackingNumber ??= reply.CompletedShipmentDetail.MasterTrackingId.TrackingNumber;
                        masterTrackingId = reply.CompletedShipmentDetail.MasterTrackingId;
                    }

                    label.InternalErrors.Add(reply.Notifications[0].Message);
                }

                return label;
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
            LabelOptions labelOptions,
            int sequenceNumber)
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
                labelOptions,
                sequenceNumber);

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
            request.RequestedShipment.DropoffType = shipment.Options.DropOffType.ToLower() switch
            {
                "regularpickup" => DropoffType.REGULAR_PICKUP,
                "dropbox" => DropoffType.DROP_BOX,
                "businessservicecenter" => DropoffType.BUSINESS_SERVICE_CENTER,
                "requestcourier" => DropoffType.REQUEST_COURIER,
                "station" => DropoffType.STATION,
                _ => DropoffType.REGULAR_PICKUP
            };

            request.RequestedShipment.PackageCount = shipment.Packages.Count().ToString();
            request.RequestedShipment.TotalWeight = new Weight()
            {
                Units = WeightUnits.LB,
                Value = shipment.Packages.Sum(x => x.Weight)
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
            var paymentType = labelOptions.PaymentType.ToLower() switch
            {
                "sender" => PaymentType.SENDER,
                "recipient" => PaymentType.RECIPIENT,
                "third_party" => PaymentType.THIRD_PARTY,
                "account" => PaymentType.ACCOUNT,
                "collect" => PaymentType.COLLECT,
                _ => PaymentType.SENDER
            };
            switch (labelOptions.PaymentType.ToLower())
            {
                case "sender":
                    request.RequestedShipment.ShippingChargesPayment = new Payment()
                    {
                        PaymentType = paymentType,
                        Payor = new Payor()
                        {
                            ResponsibleParty = new Party()
                            {
                                AccountNumber = request.ClientDetail.AccountNumber,
                            },
                        }
                    };
                    break;

                default:
                    request.RequestedShipment.ShippingChargesPayment = new Payment()
                    {
                        PaymentType = paymentType,
                        Payor = new Payor()
                        {
                            ResponsibleParty = new Party()
                            {
                                Contact = new ShipClient.v25.Contact()
                                {
                                    PersonName = shipment.Recipient.FullName,
                                    EMailAddress = shipment.Recipient.Email,
                                    PhoneNumber = shipment.Recipient.PhoneNumber
                                },
                                AccountNumber = shipment.Recipient.AccountNumber,
                            },
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
                LabelFormatType = labelOptions.LabelFormatType.ToUpper() switch
                {
                    "COMMON2D" => LabelFormatType.COMMON2D,
                    "LABEL_DATA_ONLY" => LabelFormatType.LABEL_DATA_ONLY,
                    _ => LabelFormatType.COMMON2D,
                },
                ImageType = labelOptions.ImageType.ToUpper() switch
                {
                    "PNG" => ShippingDocumentImageType.PNG,
                    "PDF" => ShippingDocumentImageType.PDF,
                    _ => ShippingDocumentImageType.PDF,
                },

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
                LabelStockType = labelOptions.LabelSize.ToLower() switch
                {
                    "4x6" => LabelStockType.PAPER_4X6,
                    "4x675" => LabelStockType.PAPER_4X675,
                    "4x8" => LabelStockType.PAPER_4X8,
                    "paper_letter" => LabelStockType.PAPER_LETTER,
                    _ => LabelStockType.PAPER_4X6
                }
            };
        }

        private void SetpackageLineItems(
            ProcessShipmentRequest request,
            Shipping.Abstractions.Models.Shipment shipment,
            LabelOptions labelOptions,
            int sequenceNumber)
        {
            request.RequestedShipment.RequestedPackageLineItems = new RequestedPackageLineItem[1];

            request.RequestedShipment.RequestedPackageLineItems[0] = new RequestedPackageLineItem()
            {
                SequenceNumber = (sequenceNumber + 1).ToString(),
                GroupPackageCount = "1",

                Weight = new Weight()
                {
                    Units = WeightUnits.LB,
                    Value = shipment.Packages[sequenceNumber].RoundedWeight,
                },

                Dimensions = new Dimensions()
                {
                    Length = shipment.Packages[sequenceNumber].Dimensions.RoundedLength.ToString(),
                    Width = shipment.Packages[sequenceNumber].Dimensions.RoundedWidth.ToString(),
                    Height = shipment.Packages[sequenceNumber].Dimensions.RoundedHeight.ToString(),
                    Units = LinearUnits.IN
                }
            };
            request.RequestedShipment.RequestedPackageLineItems[0].InsuredValue = new Money
            {
                Amount = shipment.Packages[sequenceNumber].InsuredValue,
                Currency = shipment.Options.GetCurrencyCode()
            };

            var specialServiceTypes = new string[0];
            var signatureOptionDetail = new SignatureOptionDetail
            {
                OptionType = shipment.Options.DeliverySignatureOptions.ToLower() switch
                {
                    "service_default" => SignatureOptionType.SERVICE_DEFAULT,
                    "adult" => SignatureOptionType.ADULT,
                    "direct" => SignatureOptionType.DIRECT,
                    "indirect" => SignatureOptionType.INDIRECT,
                    "nosignaturerequired" => SignatureOptionType.NO_SIGNATURE_REQUIRED,
                    _ => SignatureOptionType.SERVICE_DEFAULT
                }
            };

            if (shipment.Packages[sequenceNumber].SignatureRequiredOnDelivery &&
                signatureOptionDetail.OptionType == SignatureOptionType.NO_SIGNATURE_REQUIRED)
            {
                signatureOptionDetail.OptionType = SignatureOptionType.SERVICE_DEFAULT;
            }

            request.RequestedShipment.RequestedPackageLineItems[0].SpecialServicesRequested = new PackageSpecialServicesRequested()
            {
                SignatureOptionDetail = signatureOptionDetail,
                SpecialServiceTypes = specialServiceTypes.Append("SIGNATURE_OPTION").ToArray()
            };

            var eventTypes = new NotificationEventType[0];

            shipment.Options.EmailNotification.EmailNotificationTypes.ForEach(x =>
            {
                _ = x switch
                {
                    EmailNotificationType.On_Shipment => eventTypes.Append(NotificationEventType.ON_SHIPMENT),
                    EmailNotificationType.On_Delivery => eventTypes.Append(NotificationEventType.ON_DELIVERY),
                    EmailNotificationType.On_Estimated_Delivery => eventTypes.Append(NotificationEventType.ON_ESTIMATED_DELIVERY),
                    EmailNotificationType.On_Exception => eventTypes.Append(NotificationEventType.ON_EXCEPTION),
                    EmailNotificationType.On_Pickup_Driver_Arrived => eventTypes.Append(NotificationEventType.ON_PICKUP_DRIVER_ARRIVED),
                    EmailNotificationType.On_Pickup_Driver_Assigned => eventTypes.Append(NotificationEventType.ON_PICKUP_DRIVER_ASSIGNED),
                    EmailNotificationType.On_Pickup_Driver_Departed => eventTypes.Append(NotificationEventType.ON_PICKUP_DRIVER_DEPARTED),
                    EmailNotificationType.On_Pickup_Driver_In_Route => eventTypes.Append(NotificationEventType.ON_PICKUP_DRIVER_EN_ROUTE),
                    EmailNotificationType.On_Tender => eventTypes.Append(NotificationEventType.ON_TENDER),
                    _ => default
                };
            });

            request.RequestedShipment.SpecialServicesRequested = new ShipmentSpecialServicesRequested
            {
                SpecialServiceTypes = specialServiceTypes.Append("EVENT_NOTIFICATION").ToArray(),
                EventNotificationDetail = new ShipmentEventNotificationDetail
                {
                    // notification event message
                    PersonalMessage = shipment.Options.EmailNotification.PersonalMessage,
                    EventNotifications = new ShipmentEventNotificationSpecification[]
                    {
                        new ShipmentEventNotificationSpecification
                        {
                            Events = eventTypes.ToArray(),
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
                                    LanguageCode = shipment.Options.EmailNotification.LanguageCode
                                }
                            },
                            FormatSpecification = new ShipmentNotificationFormatSpecification
                            {
                                Type = shipment.Options.EmailNotification.NotificationFormatType.ToLower() switch
                                {
                                    "html" => NotificationFormatType.HTML,
                                    "text" => NotificationFormatType.TEXT,
                                    _ => NotificationFormatType.HTML
                                }
                            },
                            Role = shipment.Options.EmailNotification.NotificationFormatType.ToLower() switch
                            {
                                    "recipient" => ShipmentNotificationRoleType.RECIPIENT,
                                    "broker" => ShipmentNotificationRoleType.BROKER,
                                    "shipper" => ShipmentNotificationRoleType.SHIPPER,
                                    "third_party" => ShipmentNotificationRoleType.THIRD_PARTY,
                                    "other" => ShipmentNotificationRoleType.OTHER,
                                    _ => ShipmentNotificationRoleType.RECIPIENT
                            },
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
                            CustomerReferenceType = shipment.Options.CustomerReferenceType.ToLower() switch
                                                {
                                                    "customer_reference" => CustomerReferenceType.CUSTOMER_REFERENCE,
                                                    "department_number" => CustomerReferenceType.DEPARTMENT_NUMBER,
                                                    "intracountry_regulatory_reference" => CustomerReferenceType.INTRACOUNTRY_REGULATORY_REFERENCE,
                                                    "invoice_number" => CustomerReferenceType.INVOICE_NUMBER,
                                                    "po_number" => CustomerReferenceType.P_O_NUMBER,
                                                    "rma_association" => CustomerReferenceType.RMA_ASSOCIATION,
                                                    "shipment_integrity" => CustomerReferenceType.SHIPMENT_INTEGRITY,
                                                    _ => CustomerReferenceType.CUSTOMER_REFERENCE
                                                },
                            Value = request.TransactionDetail.CustomerTransactionId
                        }
                    }
                }
            };

            if (labelOptions.CollectOnDelivery.Activated)
            {
                request.RequestedShipment.SpecialServicesRequested = new ShipmentSpecialServicesRequested
                {
                    SpecialServiceTypes = specialServiceTypes.Append("COD").ToArray(),
                    CodDetail = new CodDetail()
                    {
                        CodCollectionAmount = new Money()
                        {
                            Amount = labelOptions.CollectOnDelivery.Amount,
                            Currency = labelOptions.CollectOnDelivery.Currency
                        },
                        CollectionType = labelOptions.CollectOnDelivery.CollectionType.ToUpper() switch
                        {
                            "GUARANTEED_FUNDS" => CodCollectionType.GUARANTEED_FUNDS,
                            "CASH" => CodCollectionType.CASH,
                            "ANY" => CodCollectionType.ANY,
                            "COMPANY_CHECK" => CodCollectionType.COMPANY_CHECK,
                            _ => CodCollectionType.GUARANTEED_FUNDS
                        }
                    }
                };
            }
        }

        private IEnumerable<RateRequestType> GetRateRequestTypes()
        {
            yield return RateRequestType.LIST;
        }
    }
}
