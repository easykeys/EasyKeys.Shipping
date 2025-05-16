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
                PlannedShippingDateAndTime = shipment.Options.ShippingDate.ToString("yyyy-MM-dd'T'HH:mm:ss") + " GMT+00:00",
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
                    CustomerLogos = [new CustomerLogos
                    {
                        Content = "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",
                        FileFormat = CustomerLogosFileFormat.GIF,
                    }

                    ],
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
                            AddressLine3 = "ZHAOQING, GUANDONG"
                        },
                        ContactInformation = new SupermodelIoLogisticsExpressContact
                        {
                            Email = "shipper_create_shipmentapi@dhltestmail.com",
                            Phone = "18211309039",
                            CompanyName = "Cider BookStore",
                            FullName = "LiuWeiMing"
                        }
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
                            CompanyName = "Baylee Marshall",
                            FullName = "Baylee Marshall"
                        },
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
                                    new CommodityCodes2 { TypeCode = CommodityCodes2TypeCode.Outbound, Value = "830170000000" },
                                },
                                ExportReasonType = LineItems2ExportReasonType.Commercial_purpose_or_sale,
                                ManufacturerCountry = "US",
                                Weight = new Weight4 { NetValue = 0.1, GrossValue = 0.7 },
                                IsTaxesPaid = false,
                                AdditionalInformation = new[] { "450pages" },
                                CustomerReferences = new[]
                                {
                                    new CustomerReferences3 { TypeCode = CustomerReferences3TypeCode.PON, Value = "1299210" }
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
                                    new CommodityCodes2 { TypeCode = CommodityCodes2TypeCode.Outbound, Value = "830170000000" },
                                },
                                ExportReasonType = LineItems2ExportReasonType.Commercial_purpose_or_sale,
                                ManufacturerCountry = "US",
                                Weight = new Weight4 { NetValue = 0.1, GrossValue = 0.7 },
                                IsTaxesPaid = false,
                                AdditionalInformation = new[] { "36pages" },
                                CustomerReferences = new[]
                                {
                                    new CustomerReferences3 { TypeCode = CustomerReferences3TypeCode.PON, Value = "1299211" }
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
                            SignatureImage = "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",
                            SignatureName = "Erica Bradley",
                            SignatureTitle = "Manager",
                            //IndicativeCustomsValues = new IndicativeCustomsValues2
                            //{
                            //    ImportCustomsDutyValue = 150.57,
                            //    ImportTaxesValue = 49.43
                            //},
                            CustomerReferences = new[]
                            {
                                new CustomerReferences4 { TypeCode = CustomerReferences4TypeCode.UCN, Value = "UCN-783974937" },
                                new CustomerReferences4 { TypeCode = CustomerReferences4TypeCode.CN, Value = "CUN-76498376498" },
                                new CustomerReferences4 { TypeCode = CustomerReferences4TypeCode.RMA, Value = "MyDHLAPI-TESTREF-001" }
                            }
                        },
                        Remarks = new[] { new Remarks2 { Value = "Right side up only" } },
                        //AdditionalCharges = new[]
                        //{
                        //    new AdditionalCharges2 { TypeCode = AdditionalCharges2TypeCode.Freight, Caption = "fee", Value = 10 },
                        //    new AdditionalCharges2 { TypeCode = AdditionalCharges2TypeCode.Other, Caption = "freight charges", Value = 20 },
                        //    new AdditionalCharges2 { TypeCode = AdditionalCharges2TypeCode.Insurance, Caption = "ins charges", Value = 10 },
                        //    new AdditionalCharges2 { TypeCode = AdditionalCharges2TypeCode.Reverse_charge, Caption = "rev charges", Value = 7 }
                        //},
                        //DestinationPortName = "New York Port",
                        //PlaceOfIncoterm = "ShenZhen Port",
                        //PayerVATNumber = "12345ED",
                        RecipientReference = "01291344",
                        //Exporter = new Exporter2 { Id = "121233", Code = "S" },
                        PackageMarks = "Fragile glass bottle",
                        DeclarationNotes = new[] { new DeclarationNotes { Value = "up to three declaration notes" } },
                        ExportReference = "export reference",
                        ExportReason = "export reason",
                        ExportReasonType = ExportDeclarationExportReasonType.Commercial_purpose_or_sale,
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
            var nonDiscountCharges = result.ShipmentCharges.First(x => x.CurrencyType == "PULCL");
            foreach (var doc in result.Documents.Where(x => x.TypeCode == ImageOptionsTypeCode.Label.ToString().ToLower()))
            {
                label.Labels.Add(new PackageLabelDetails
                {
                    Bytes = new () { Convert.FromBase64String(doc.Content) },
                    ImageType = doc.ImageFormat,
                    TotalCharges = new Shipping.Abstractions.Models.ShipmentCharges
                    {
                        NetCharge = (decimal)billCharges.Price,
                        SurchargesList = billCharges.ServiceBreakdown.Where(x => x.TypeCode != null).ToDictionary(x => x.Name, x => (decimal)x.Price),
                        Surcharges = (decimal)billCharges.ServiceBreakdown.Where(x => x.TypeCode != null).Sum(x => x.Price),
                        BaseCharge = (decimal)billCharges.ServiceBreakdown.Where(x => x.TypeCode == null).Sum(x => x.Price)
                    },
                    TotalCharges2 = new Shipping.Abstractions.Models.ShipmentCharges
                    {
                        NetCharge = (decimal)nonDiscountCharges.Price,
                        SurchargesList = nonDiscountCharges.ServiceBreakdown.Where(x => x.TypeCode != null).ToDictionary(x => x.Name, x => (decimal)x.Price),
                        Surcharges = (decimal)nonDiscountCharges.ServiceBreakdown.Where(x => x.TypeCode != null).Sum(x => x.Price),
                        BaseCharge = (decimal)nonDiscountCharges.ServiceBreakdown.Where(x => x.TypeCode == null).Sum(x => x.Price)
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
