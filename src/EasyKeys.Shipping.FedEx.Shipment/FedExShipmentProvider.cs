using EasyKeys.Shipping.Abstractions.Extensions;
using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.FedEx.Abstractions.Models;
using EasyKeys.Shipping.FedEx.Abstractions.Options;
using EasyKeys.Shipping.FedEx.Shipment.Extensions;
using EasyKeys.Shipping.FedEx.Shipment.Models;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using ShipClient.v25;

namespace EasyKeys.Shipping.FedEx.Shipment;

public class FedExShipmentProvider : IFedExShipmentProvider
{
    private readonly ILogger<FedExShipmentProvider> _logger;
    private FedExOptions _options;

    public FedExShipmentProvider(
        IOptionsMonitor<FedExOptions> optionsMonitor,
        ILogger<FedExShipmentProvider> logger)
    {
        _options = optionsMonitor.CurrentValue;

        optionsMonitor.OnChange(x => _options = x);

        _logger = logger ?? throw new ArgumentException(nameof(logger));
    }

    public async Task<ShipmentLabel> CreateShipmentAsync(
        ServiceType serviceType,
        Shipping.Abstractions.Models.Shipment shipment,
        ShipmentDetails shipmentDetails,
        CancellationToken cancellationToken = default)
    {
        var client = new ShipPortTypeClient(
            ShipPortTypeClient.EndpointConfiguration.ShipServicePort,
            _options.Url);

        var label = new ShipmentLabel();
        try
        {
            // for multiple packages, each package must have its own request.
            for (var i = 0; i < shipment.Packages.Count; i++)
            {
                var request = CreateShipmentRequest(
                        serviceType,
                        shipment,
                        shipmentDetails,
                        i);

                var shipmentRequest = new processShipmentRequest1(request);

                var response = await client.processShipmentAsync(shipmentRequest);

                var reply = response?.ProcessShipmentReply;

                if ((reply?.HighestSeverity != NotificationSeverityType.ERROR)
                         && (reply?.HighestSeverity != NotificationSeverityType.FAILURE))
                {
                    var charges = new PackageCharges();

                    var packageDetails = reply?.CompletedShipmentDetail?.CompletedPackageDetails?.ToList();

                    reply?.CompletedShipmentDetail.CompletedPackageDetails.ToList().
                        ForEach(x =>
                        {
                            charges.Surcharges = new Dictionary<string, decimal>();

                            charges.BaseCharge = x.PackageRating.PackageRateDetails.Select(x => x.BaseCharge).Sum(s => s.Amount);
                            charges.NetCharge = x.PackageRating.PackageRateDetails.Select(x => x.NetCharge).Sum(s => s.Amount);

                            x.PackageRating.PackageRateDetails
                                                .SelectMany(x => x.Surcharges)
                                                .ToList()
                                                .ForEach(x => charges.Surcharges[x.Description] = x.Amount.Amount);

                            charges.TotalSurCharges = x.PackageRating.PackageRateDetails.Select(x => x.TotalSurcharges).Sum(s => s.Amount);

                            label.Labels.Add(
                                new PackageLabelDetails
                                {
                                    Charges = charges,

                                    TrackingId = x.TrackingIds.Select(x => x.TrackingNumber).Flatten(";"),

                                    ImageType = x.Label.ImageType.ToString(),

                                    Bytes = x.Label.Parts.Select(x => x.Image).ToList()
                                });
                        });
                }
                else
                {
                    label.InternalErrors.Add(reply.Notifications.Select(x => x.Message).Flatten(","));
                }
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
        ShipmentDetails details,
        int sequenceNumber)
    {
        var request = CreateRequest(details);

        SetShipmentDetails(
            request,
            serviceType,
            shipment,
            details);

        SetpackageLineItems(
            request,
            shipment,
            details,
            sequenceNumber);

        return request;
    }

    private ProcessShipmentRequest CreateRequest(ShipmentDetails details)
    {
        return new ProcessShipmentRequest
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
                CustomerTransactionId = details.TransactionId
            },
            Version = new VersionId()
        };
    }

    private void SetShipmentDetails(
        ProcessShipmentRequest request,
        ServiceType serviceType,
        Shipping.Abstractions.Models.Shipment shipment,
        ShipmentDetails details)
    {
        request.RequestedShipment = new RequestedShipment
        {
            ShipTimestamp = shipment.Options.ShippingDate ?? DateTime.Now,
            ServiceType = serviceType.ToString(),
            PackagingType = shipment.Options.PackagingType,
            PackageCount = shipment.Packages.Count.ToString(),
            TotalWeight = new Weight
            {
                Value = shipment.Packages.Sum(x => x.Weight),
                Units = WeightUnits.LB
            },

            RateRequestTypes = details.RateRequestType.ToLower() switch
            {
                "none" => new RateRequestType[1] { RateRequestType.NONE },
                "list" => new RateRequestType[1] { RateRequestType.LIST },
                "preferred" => new RateRequestType[1] { RateRequestType.PREFERRED },
            }
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

        request.RequestedShipment.PackageCount = shipment.Packages.Count.ToString();
        request.RequestedShipment.TotalWeight = new Weight()
        {
            Units = WeightUnits.LB,
            Value = shipment.Packages.Sum(x => x.Weight)
        };

        SetSender(request, details.Sender, shipment);

        SetRecipient(request, details.Recipient, shipment);

        SetPayment(request, shipment, details);

        SetLabelDetails(request, shipment, details);
    }

    private void SetSender(
        ProcessShipmentRequest request,
        SenderContact shipper,
        Shipping.Abstractions.Models.Shipment shipment)
    {
        request.RequestedShipment.Shipper = new Party
        {
            Contact = new ShipClient.v25.Contact()
            {
                PersonName = shipper.FullName,
                CompanyName = shipper.CompanyName,
                PhoneNumber = shipper.PhoneNumber,
                EMailAddress = shipper.Email
            },
            Address = shipment.OriginAddress.GetFedExAddress()
        };
    }

    private void SetRecipient(
        ProcessShipmentRequest request,
        RecipientContact recipient,
        Shipping.Abstractions.Models.Shipment shipment)
    {
        request.RequestedShipment.Recipient = new Party
        {
            Contact = new ShipClient.v25.Contact()
            {
                PersonName = recipient.FullName,
                EMailAddress = recipient.Email,
                PhoneNumber = recipient.PhoneNumber
            },
            Address = shipment.DestinationAddress.GetFedExAddress()
        };
    }

    private void SetPayment(
        ProcessShipmentRequest request,
        Shipping.Abstractions.Models.Shipment shipment,
        ShipmentDetails details)
    {
        var paymentType = details.PaymentType.ToLower() switch
        {
            "sender" => PaymentType.SENDER,
            "recipient" => PaymentType.RECIPIENT,
            "third_party" => PaymentType.THIRD_PARTY,
            "account" => PaymentType.ACCOUNT,
            "collect" => PaymentType.COLLECT,
            _ => PaymentType.SENDER
        };

        switch (paymentType)
        {
            case PaymentType.SENDER:
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
            case PaymentType.ACCOUNT:
            case PaymentType.COLLECT:
            case PaymentType.RECIPIENT:
            case PaymentType.THIRD_PARTY:
                request.RequestedShipment.ShippingChargesPayment = new Payment()
                {
                    PaymentType = paymentType,
                    Payor = new Payor()
                    {
                        ResponsibleParty = new Party()
                        {
                            Contact = new ShipClient.v25.Contact()
                            {
                                PersonName = details.Recipient.FullName,
                                EMailAddress = details.Recipient.Email,
                                PhoneNumber = details.Recipient.PhoneNumber
                            },
                            AccountNumber = details.Recipient.AccountNumber,
                        },
                    }
                };
                break;
        }
    }

    private void SetLabelDetails(
        ProcessShipmentRequest request,
        Shipping.Abstractions.Models.Shipment shipment,
        ShipmentDetails details)
    {
        request.RequestedShipment.LabelSpecification = new LabelSpecification
        {
            LabelFormatType = details.LabelOptions.LabelFormatType.ToUpper() switch
            {
                "COMMON2D" => LabelFormatType.COMMON2D,
                "LABEL_DATA_ONLY" => LabelFormatType.LABEL_DATA_ONLY,
                _ => LabelFormatType.COMMON2D,
             },

            ImageType = details.LabelOptions.ImageType.ToUpper() switch
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
                    PersonName = details.Sender.FullName,
                    EMailAddress = details.Sender.Email,
                    PhoneNumber = details.Sender.PhoneNumber
                },
                Address = shipment.OriginAddress.GetFedExAddress()
            },

            LabelStockType = details.LabelOptions.LabelSize.ToLower() switch
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
        ShipmentDetails details,
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
            OptionType = details.DeliverySignatureOptions.ToLower() switch
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

        details.EmailNotification.EmailNotificationTypes.ToList().ForEach(x =>
        {
            _ = x switch
            {
                "On_Shipment" => eventTypes.Append(NotificationEventType.ON_SHIPMENT),
                "On_Delivery" => eventTypes.Append(NotificationEventType.ON_DELIVERY),
                "On_Estimated_Delivery" => eventTypes.Append(NotificationEventType.ON_ESTIMATED_DELIVERY),
                "On_Exception" => eventTypes.Append(NotificationEventType.ON_EXCEPTION),
                "On_Pickup_Driver_Arrived" => eventTypes.Append(NotificationEventType.ON_PICKUP_DRIVER_ARRIVED),
                "On_Pickup_Driver_Assigned" => eventTypes.Append(NotificationEventType.ON_PICKUP_DRIVER_ASSIGNED),
                "On_Pickup_Driver_Departed" => eventTypes.Append(NotificationEventType.ON_PICKUP_DRIVER_DEPARTED),
                "On_Pickup_Driver_In_Route" => eventTypes.Append(NotificationEventType.ON_PICKUP_DRIVER_EN_ROUTE),
                "On_Tender" => eventTypes.Append(NotificationEventType.ON_TENDER),
                _ => default
            };
        });

        request.RequestedShipment.SpecialServicesRequested = new ShipmentSpecialServicesRequested
        {
            SpecialServiceTypes = specialServiceTypes.Append("EVENT_NOTIFICATION").ToArray(),
            EventNotificationDetail = new ShipmentEventNotificationDetail
            {
                // notification event message
                PersonalMessage = details.EmailNotification.PersonalMessage,
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
                                EmailAddress = details.Recipient.Email,
                                Name = details.Recipient.FullName,
                            },
                            Localization = new Localization
                            {
                                LanguageCode = details.EmailNotification.LanguageCode
                            }
                        },
                        FormatSpecification = new ShipmentNotificationFormatSpecification
                        {
                            Type = details.EmailNotification.NotificationFormatType.ToLower() switch
                            {
                                "html" => NotificationFormatType.HTML,
                                "text" => NotificationFormatType.TEXT,
                                _ => NotificationFormatType.HTML
                            }
                        },
                        Role = details.EmailNotification.NotificationRoleType.ToLower() switch
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
                        CustomerReferenceType = details.CustomerReferenceType.ToLower() switch
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

        if (details.CollectOnDelivery.Activated)
        {
            request.RequestedShipment.SpecialServicesRequested = new ShipmentSpecialServicesRequested
            {
                SpecialServiceTypes = specialServiceTypes.Append("COD").ToArray(),
                CodDetail = new CodDetail()
                {
                    CodCollectionAmount = new Money()
                    {
                        Amount = details.CollectOnDelivery.Amount,
                        Currency = shipment.Options.PreferredCurrencyCode
                    },
                    CollectionType = details.CollectOnDelivery.CollectionType.ToUpper() switch
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
}
