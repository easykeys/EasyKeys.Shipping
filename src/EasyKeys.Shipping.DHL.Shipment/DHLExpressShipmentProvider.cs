using System.Text;

using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.DHL.Abstractions.OpenApis.V2.Express;
using EasyKeys.Shipping.DHL.Abstractions.Options;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EasyKeys.Shipping.DHL.Shipment;

public class DHLExpressShipmentProvider : IDHLExpressShipmentProvider
{
    private readonly DHLExpressApi _dHLExpressApi;
    private readonly DHLExpressApiOptions _dHLExpressApiOptions;
    private readonly ILogger<DHLExpressShipmentProvider> _logger;

    public DHLExpressShipmentProvider(
        IOptionsMonitor<DHLExpressApiOptions> dHLExpressApiOptions,
        DHLExpressApi dHLExpressApi,
        ILogger<DHLExpressShipmentProvider> logger)
    {
       _dHLExpressApiOptions = dHLExpressApiOptions?.CurrentValue ?? throw new ArgumentNullException(nameof(dHLExpressApiOptions));
       _dHLExpressApi = dHLExpressApi ?? throw new ArgumentNullException(nameof(dHLExpressApi));
       _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ShipmentLabel> CreateShipmentAsync(Shipping.Abstractions.Models.Shipment shipment, CancellationToken cancellationToken = default)
    {
        var label = new ShipmentLabel();
        try
        {
         
            var body = new SupermodelIoLogisticsExpressCreateShipmentRequest
            {
                PlannedShippingDateAndTime = "2025-05-19T19:19:40 GMT+00:00",
                Pickup = new Pickup { IsRequested = false },
                ProductCode = "P",
                LocalProductCode = "P",
                GetRateEstimates = true,
                Accounts = new[]
                {
                    new SupermodelIoLogisticsExpressAccount
                    {
                        Number = _dHLExpressApiOptions.AccountNumber,
                        TypeCode = SupermodelIoLogisticsExpressAccountTypeCode.Shipper
                    }
                },
                ValueAddedServices = new[]
                {
                    new SupermodelIoLogisticsExpressValueAddedServices
                    {
                        ServiceCode = "II",
                        Value = 10,
                        Currency = "USD"
                    }
                },
                OutputImageProperties = new OutputImageProperties
                {
                    PrinterDPI = PrinterDPI.DPI300,
                    EncodingFormat = OutputImagePropertiesEncodingFormat.Pdf,
                    ImageOptions = new[]
                    {
                        new ImageOptions
                        {
                            TypeCode = ImageOptionsTypeCode.Invoice,
                            TemplateName = "COMMERCIAL_INVOICE_P_10",
                            IsRequested = true,
                            InvoiceType = ImageOptionsInvoiceType.Commercial,
                            LanguageCode = "eng",
                            LanguageCountryCode = "US"
                        },
                        new ImageOptions
                        {
                            TypeCode = ImageOptionsTypeCode.WaybillDoc,
                            TemplateName = "ARCH_8x4",
                            IsRequested = true,
                            HideAccountNumber = false,
                            NumberOfCopies = 1
                        },
                        new ImageOptions
                        {
                            TypeCode = ImageOptionsTypeCode.Label,
                            TemplateName = "ECOM26_84_001",
                            RenderDHLLogo = true,
                            FitLabelsToA4 = false
                        }
                    },
                    SplitTransportAndWaybillDocLabels = false,
                    AllDocumentsInOneImage = false,
                    SplitDocumentsByPages = false,
                    SplitInvoiceAndReceipt = true,
                    ReceiptAndLabelsInOneImage = false
                },
                CustomerDetails = new CustomerDetails
                {
                    ReceiverDetails = new ReceiverDetails
                    {
                        PostalAddress = new SupermodelIoLogisticsExpressAddressCreateShipmentRequest
                        {
                            PostalCode = "526238",
                            CityName = "Zhaoqing",
                            CountryCode = "CN",
                            AddressLine1 = "4FENQU, 2HAOKU, WEIPINHUI WULIU YUAN，DAWANG",
                            AddressLine2 = "GAOXIN QU, BEIJIANG DADAO, SIHUI,",
                            AddressLine3 = "ZHAOQING, GUANDONG",
                            CountyName = "SIHUI",
                            CountryName = "CHINA, PEOPLES REPUBLIC"
                        },
                        ContactInformation = new SupermodelIoLogisticsExpressContact
                        {
                            Email = "shipper_create_shipmentapi@dhltestmail.com",
                            Phone = "18211309039",
                            MobilePhone = "18211309039",
                            CompanyName = "Cider BookStore",
                            FullName = "LiuWeiMing"
                        },
                        RegistrationNumbers = new[]
                        {
                            new SupermodelIoLogisticsExpressRegistrationNumbers
                            {
                                TypeCode = SupermodelIoLogisticsExpressRegistrationNumbersTypeCode.SDT,
                                Number = "CN123456789",
                                IssuerCountryCode = "CN"
                            }
                        },
                        BankDetails = [new ()
                            {
                                Name = "Bank of China",
                                SettlementLocalCurrency = "RMB",
                                SettlementForeignCurrency = "USD"
                            }

                        ],

                        TypeCode = ReceiverDetailsTypeCode.Business
                    },
                    ShipperDetails = new ShipperDetails
                    {
                        PostalAddress = new SupermodelIoLogisticsExpressAddressCreateShipmentRequest
                        {
                            PostalCode = "76449",
                            CityName = "Graford",
                            CountryCode = "US",
                            AddressLine1 = "116 Marine Dr",
                            CountryName = "UNITED STATES OF AMERICA"
                        },
                        ContactInformation = new SupermodelIoLogisticsExpressContact
                        {
                            Email = "recipient_create_shipmentapi@dhltestmail.com",
                            Phone = "9402825665",
                            MobilePhone = "9402825666",
                            CompanyName = "Baylee Marshall",
                            FullName = "Baylee Marshall"
                        },
                        RegistrationNumbers = new[]
                        {
                            new SupermodelIoLogisticsExpressRegistrationNumbers
                            {
                                TypeCode = SupermodelIoLogisticsExpressRegistrationNumbersTypeCode.SSN,
                                Number = "US123456789",
                                IssuerCountryCode = "US"
                            }
                        },
                        BankDetails = [new ()
                            {
                                Name = "Bank of America",
                                SettlementLocalCurrency = "USD",
                                SettlementForeignCurrency = "USD"
                            }

                        ],

                        TypeCode = ShipperDetailsTypeCode.Business
                    }
                },
                Content = new Content2
                {
                    IsCustomsDeclarable = true,
                    DeclaredValue = 120,
                    DeclaredValueCurrency = "USD",
                    Description = "Shipment",
                    Incoterm = Content2Incoterm.DAP,
                    UnitOfMeasurement = Content2UnitOfMeasurement.Metric,
                    Packages = new[]
                    {
                        new SupermodelIoLogisticsExpressPackage
                        {
                            TypeCode = SupermodelIoLogisticsExpressPackageTypeCode._2BP,
                            Weight = 0.5,
                            Dimensions = new Dimensions2
                            {
                                Length = 1,
                                Width = 1,
                                Height = 1
                            },
                            CustomerReferences = [ new ()
                                {
                                    TypeCode = SupermodelIoLogisticsExpressPackageReferenceTypeCode.CU,
                                    Value = "3654673"
                                }

                            ],
                            Description = "Piece content description",
                            LabelDescription = "bespoke label description"
                        }
                    },
                    ExportDeclaration = new ExportDeclaration
                    {
                        LineItems = new[]
                        {
                            new LineItems2
                            {
                                Number = 1,
                                Description = "Harry Steward biography first edition",
                                Price = 15,
                                Quantity = new Quantity2 { Value = 4, UnitOfMeasurement = Quantity2UnitOfMeasurement.GM },
                                CommodityCodes = new[]
                                {
                                    new CommodityCodes2 { TypeCode = CommodityCodes2TypeCode.Outbound, Value = "84713000" },
                                    new CommodityCodes2 { TypeCode = CommodityCodes2TypeCode.Inbound, Value = "5109101110" }
                                },
                                ExportReasonType = LineItems2ExportReasonType.Permanent,
                                ManufacturerCountry = "US",
                               //ExportControlClassificationNumber = "US123456789",
                                Weight = new Weight4 { NetValue = 0.1, GrossValue = 0.7 },
                                IsTaxesPaid = true,
                                AdditionalInformation = new[] { "450pages" },
                                CustomerReferences = new[]
                                {
                                    new CustomerReferences3 { TypeCode = CustomerReferences3TypeCode.AFE, Value = "1299210" }
                                },
                                CustomsDocuments = new[]
                                {
                                    new CustomsDocuments4 { TypeCode = CustomsDocuments4TypeCode.COO, Value = "MyDHLAPI - LN#1-CUSDOC-001" }
                                }
                            },
                            new LineItems2
                            {
                                Number = 2,
                                Description = "Andromeda Chapter 394 - Revenge of Brook",
                                Price = 15,
                                Quantity = new Quantity2 { Value = 4, UnitOfMeasurement = Quantity2UnitOfMeasurement.GM },
                                CommodityCodes = new[]
                                {
                                    new CommodityCodes2 { TypeCode = CommodityCodes2TypeCode.Outbound, Value = "6109100011" },
                                    new CommodityCodes2 { TypeCode = CommodityCodes2TypeCode.Inbound, Value = "5109101111" }
                                },
                                ExportReasonType = LineItems2ExportReasonType.Permanent,
                                ManufacturerCountry = "US",
                               // ExportControlClassificationNumber = "US123456789",
                                Weight = new Weight4 { NetValue = 0.1, GrossValue = 0.7 },
                                IsTaxesPaid = true,
                                AdditionalInformation = new[] { "36pages" },
                                CustomerReferences = new[]
                                {
                                    new CustomerReferences3 { TypeCode = CustomerReferences3TypeCode.AFE, Value = "1299211" }
                                },
                                CustomsDocuments = new[]
                                {
                                    new CustomsDocuments4 { TypeCode = CustomsDocuments4TypeCode.COO, Value = "MyDHLAPI - LN#1-CUSDOC-001" }
                                }
                            }
                        },
                        Invoice = new Invoice2
                        {
                            Number = "2667168671",
                            Date = DateTime.Parse("2025-05-15"),
                            Instructions = new[] { "Handle with care" },
                            TotalNetWeight = 0.4,
                            TotalGrossWeight = 0.5,
                            TermsOfPayment = "100 days",
                            IndicativeCustomsValues = new IndicativeCustomsValues2
                            {
                                ImportCustomsDutyValue = 150.57,
                                ImportTaxesValue = 49.43
                            },
                            CustomerReferences = new[]
                            {
                                new CustomerReferences4 { TypeCode = CustomerReferences4TypeCode.UCN, Value = "UCN-783974937" },
                                new CustomerReferences4 { TypeCode = CustomerReferences4TypeCode.CN, Value = "CUN-76498376498" },
                                new CustomerReferences4 { TypeCode = CustomerReferences4TypeCode.RMA, Value = "MyDHLAPI-TESTREF-001" }
                            }
                        },
                        Remarks = new[] { new Remarks2 { Value = "Right side up only" } },
                        AdditionalCharges = new[]
                        {
                            new AdditionalCharges2 { TypeCode = AdditionalCharges2TypeCode.Freight, Caption = "fee", Value = 10 },
                            new AdditionalCharges2 { TypeCode = AdditionalCharges2TypeCode.Other, Caption = "freight charges", Value = 20 },
                            new AdditionalCharges2 { TypeCode = AdditionalCharges2TypeCode.Insurance, Caption = "ins charges", Value = 10 },
                            new AdditionalCharges2 { TypeCode = AdditionalCharges2TypeCode.Reverse_charge, Caption = "rev charges", Value = 7 }
                        },
                        DestinationPortName = "New York Port",
                        PlaceOfIncoterm = "ShenZhen Port",
                        PayerVATNumber = "12345ED",
                        RecipientReference = "01291344",
                        Exporter = new Exporter2 { Id = "121233", Code = "S" },
                        PackageMarks = "Fragile glass bottle",
                        DeclarationNotes = new[] { new DeclarationNotes { Value = "up to three declaration notes" } },
                        ExportReference = "export reference",
                        ExportReason = "export reason",
                        ExportReasonType = ExportDeclarationExportReasonType.Permanent,
                        Licenses = new[] { new Licenses { TypeCode = LicensesTypeCode.Export, Value = "123127233" } },
                        ShipmentType = ExportDeclarationShipmentType.Personal,
                        CustomsDocuments = new[]
                            {
                                new CustomsDocuments3 { TypeCode = CustomsDocuments3TypeCode.INV, Value = "MyDHLAPI - CUSDOC-001" }
                            }
                    }
                },
                ShipmentNotification = new[]
                {
                    new ShipmentNotification
                    {
                        TypeCode = ShipmentNotificationTypeCode.Email,
                        ReceiverId = "shipmentnotification@mydhlapisample.com",
                        LanguageCode = "eng",
                        LanguageCountryCode = "UK",
                        BespokeMessage = "message to be included in the notification"
                    }
                },
                EstimatedDeliveryDate = new EstimatedDeliveryDate
                    {
                        IsRequested = true,
                        TypeCode = EstimatedDeliveryDateTypeCode.QDDC
                    },
                GetAdditionalInformation = new[]
                {
                    new GetAdditionalInformation
                    {
                        TypeCode = GetAdditionalInformationTypeCode.PickupDetails,
                        IsRequested = true
                    }
                },
                GetTransliteratedResponse = false
            };

            var result = await _dHLExpressApi.ExpApiShipmentsAsync(body, cancellationToken: cancellationToken);

            var billCharges = result.ShipmentCharges.First(x => x.CurrencyType == "BILLC");
            foreach (var doc in result.Documents.Where(x => x.TypeCode == ImageOptionsTypeCode.Label.ToString().ToLower()))
            {
                label.Labels.Add(new PackageLabelDetails
                {
                    Bytes = new() { Convert.FromBase64String(doc.Content) },
                    ImageType = doc.ImageFormat,
                    TotalCharges = new Shipping.Abstractions.Models.ShipmentCharges
                    {
                        NetCharge = (decimal)billCharges.Price,
                        SurchargesList = billCharges.ServiceBreakdown.ToDictionary(x => x.Name, x => (decimal)x.Price)
                    },
                    ProviderLabelId = result.TrackingUrl,
                    TrackingId = result.ShipmentTrackingNumber
                });
            }

            foreach (var doc in result.Documents.Where(x => x.TypeCode == ImageOptionsTypeCode.Invoice.ToString().ToLower()))
            {
                label.ShippingDocuments.Add(new Document()
                {
                    Bytes = new() { Convert.FromBase64String(doc.Content) },
                    ImageType = doc.ImageFormat,
                    DocumentName = doc.TypeCode,
                    CopiesToPrint = "1"
                });
            }
        }
        catch (ApiException<SupermodelIoLogisticsExpressErrorResponse> ex)
        {
            var error = ex?.Result.Detail ?? string.Empty;
            _logger.LogError("{name} : {message}", nameof(DHLExpressShipmentProvider), error);
            label.InternalErrors.Add(error);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{providerName} failed", nameof(DHLExpressShipmentProvider));
            label.InternalErrors.Add(ex?.Message ?? $"{nameof(DHLExpressShipmentProvider)} failed");
        }

        return label;
    }
}
