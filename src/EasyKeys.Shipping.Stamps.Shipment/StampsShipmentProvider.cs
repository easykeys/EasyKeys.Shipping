using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Abstractions.Services;

using StampsClient.v111;

namespace EasyKeys.Shipping.Stamps.Shipment
{
    public class StampsShipmentProvider : IStampsShipmentProvider
    {
        private readonly IStampsClientService _stampsClient;
        private readonly IGetRatesV40 _ratesV40;

        public StampsShipmentProvider(IStampsClientService stampsClientService, IGetRatesV40 ratesV40)
        {
            _stampsClient = stampsClientService;
            _ratesV40 = ratesV40;
        }

        public async Task<ShipmentLabel> CreateShipmentAsync(
            Shipping.Abstractions.Models.Shipment shipment,
            ShipmentDetails details,
            Abstractions.Models.ServiceType serviceType,
            CancellationToken cancellationToken)
        {
            var client = _stampsClient.CreateClient();

            var rates = await _ratesV40.GetRatesResponseAsync(shipment, details, serviceType, cancellationToken);

            var request = new CreateIndiciumRequest()
            {
                Item = await _stampsClient.GetTokenAsync(cancellationToken),

                IntegratorTxID = Guid.NewGuid().ToString(),

                Rate = rates?.FirstOrDefault(),

                ReturnTo = rates?.FirstOrDefault()?.From,

                CustomerID = Guid.NewGuid().ToString(),

                Customs = shipment.Commodities.Any() ? SetCustomsInformation(shipment, details) : default,

                SampleOnly = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") == "Development" ? true : false,

                /*Normal	A regular label with postage and a valid indicium.
                 NoPostage	A regular label without postage or an indicium.
                */
                PostageMode = PostageMode.Normal,

                ImageType = details.LabelOptions.ImageType switch
                {
                    "png" => ImageType.Png,
                    "pdf" => ImageType.Pdf,
                    _ => ImageType.Png
                },

                EltronPrinterDPIType = details.LabelOptions.Resolution switch
                {
                    "default" => EltronPrinterDPIType.Default,
                    "high" => EltronPrinterDPIType.High,
                    _ => EltronPrinterDPIType.Default
                },

                memo = details.LabelOptions.Memo,

                // ??
                cost_code_id = 0,

                deliveryNotification = true,

                ShipmentNotification = SetShipmentNotification(shipment),

                rotationDegrees = details.LabelOptions.RotationDegrees,

                horizontalOffset = details.LabelOptions.HorizontalOffet,

                verticalOffset = details.LabelOptions.VerticalOffet,

                printDensity = details.LabelOptions.PrintDensity,

                printMemo = string.IsNullOrEmpty(details.LabelOptions.Memo) ? false : true,

                printInstructions = details.LabelOptions.PrintInstructions,

                // ??
                requestPostageHash = false,

                // Specifies the delivery options for use in the CP72 customs form.
                nonDeliveryOption = NonDeliveryOption.Undefined,

                // If nonDeliveryOption is Redirect, this address is printed on the CP72 customs form; otherwise, this address is ignored.
                RedirectTo = null,

                // ?
                OutboundTransactionID = String.Empty,

                // ?
                OriginalPostageHash = String.Empty,

                ReturnImageData = true,

                /* For International mail, shipments valued over $2500 generally require an
                    Internal Transaction Number (ITN) to be put on the CP72 form.*/
                InternalTransactionNumber = "123",

                PaperSize = details.LabelOptions.LabelSize.ToLower() switch
                {
                    "4x6" => PaperSizeV1.Default,
                    _ => PaperSizeV1.Default
                },

                EmailLabelTo = string.IsNullOrEmpty(details.LabelOptions.EmailLabelTo) ? null : SetEmailLabelTo(details),

                // ??
                PayOnPrint = false,

                // ??
                ReturnLabelExpirationDays = 1,

                ImageDpi = details.LabelOptions.ImageDPI switch
                {
                    "ImageDpi203" => ImageDpi.ImageDpiDefault,
                    "ImageDpi300" => ImageDpi.ImageDpi300,
                    "ImageDpi200" => ImageDpi.ImageDpi200,
                    "ImageDpi150" => ImageDpi.ImageDpi150,
                    "ImageDpi96" => ImageDpi.ImageDpi96,
                    _ => ImageDpi.ImageDpiDefault
                },

                // ??
                RateToken = string.Empty,

                // Caller defined data. Order ID associated to this print.
                OrderId = details.OrderId,

                // ?
                BypassCleanseAddress = false,

                // Identifies the image or logo that will be added to the shipping label.
                ImageId = 0,

                // Caller defined data. 1 - 4
                Reference1 = string.Empty,

                Reference2 = string.Empty,

                Reference3 = string.Empty,

                Reference4 = string.Empty,

                // ??
                ReturnIndiciumData = false,

                // ??
                //ExtendedPostageInfo = new ExtendedPostageInfoV1(),

                // ??
                EnclosedServiceType = EnclosedServiceType.Unknown,

                // ??
                EnclosedPackageType = EnclosedPackageType.Unknown,

                //Caller defined data. Specifies the branding to be used for emails corresponding with this print.
                BrandingId = Guid.NewGuid(),
            };

            request = SetOrderDetails(request, shipment);

            try
            {
                var response = await client.CreateIndiciumAsync(request);

                return new ShipmentLabel()
                {
                    Labels = new List<PackageLabelDetails>()
                    {
                        new PackageLabelDetails()
                        {
                            ImageType = ImageType.Png.ToString(),
                            TrackingId = response.TrackingNumber.ToString(),
                            Bytes = response.ImageData.ToList(),
                        }
                    }
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<CancelIndiciumResponse> CancelShipmentAsync(ShipmentLabel shipmentLabel, CancellationToken cancellationToken)
        {
            var client = _stampsClient.CreateClient();

            var request = new CancelIndiciumRequest()
            {
                Item = await _stampsClient.GetTokenAsync(cancellationToken),
                Item1 = shipmentLabel.Labels[0].ProviderLabelId
            };
            try
            {
                return await client.CancelIndiciumAsync(request);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private CustomsV7 SetCustomsInformation(Shipping.Abstractions.Models.Shipment shipment, ShipmentDetails details)
        {
            // using shipment.commodities information set request.customs

            var customsLine = new List<CustomsLine>();

            foreach (var commodity in shipment.Commodities)
            {
                customsLine.Add(new CustomsLine
                {
                    Description = commodity.Description,
                    CountryOfOrigin = commodity.CountryOfManufacturer,
                    Quantity = commodity.Quantity,
                    HSTariffNumber = string.Empty,
                    sku = commodity.PartNumber,
                    Value = commodity.CustomsValue,
                    WeightLb = (double)commodity.Weight,
                    WeightOz = 0.0d
                });
            }

            return new CustomsV7()
            {
                ContentType = details.ContentType.ToLower() switch
                {
                    "commercial_sample" => ContentTypeV2.CommercialSample,
                    "dangerous_goods" => ContentTypeV2.DangerousGoods,
                    "document" => ContentTypeV2.Document,
                    "gift" => ContentTypeV2.Gift,
                    "humanitarian" => ContentTypeV2.HumanitarianDonation,
                    "merchandise" => ContentTypeV2.Merchandise,
                    "returned_goods" => ContentTypeV2.ReturnedGoods,
                    _ => ContentTypeV2.Other
                },

                Comments = shipment?.Commodities?.FirstOrDefault()?.Comments,

                LicenseNumber = shipment?.Commodities?.FirstOrDefault()?.ExportLicenseNumber,

                CertificateNumber = String.Empty,

                InvoiceNumber = String.Empty,

                OtherDescribe = String.Empty,

                CustomsLines = customsLine.ToArray(),

                CustomsSigner = string.Empty,

                PassportNumber = String.Empty,

                // PassportIssueDate =

                // PassportExpiryDate =

                ImportLicenseNumber = String.Empty,

                SendersCustomsReference = String.Empty
            };
        }

        private ShipmentNotification SetShipmentNotification(Shipping.Abstractions.Models.Shipment shipment)
        {
            // using shipment informtion to set request.shipmentNotification
            return new ShipmentNotification()
            {
                Email = shipment.RecipientContact.Email,
                CCToAccountHolder = true,
                UseCompanyNameInFromLine = true,
                UseCompanyNameInSubject = true,
            };
        }

        private LabelRecipientInfo SetEmailLabelTo(ShipmentDetails details)
        {
            return new LabelRecipientInfo()
            {
                EmailAddress = details.LabelOptions.EmailLabelTo
            };
        }

        private CreateIndiciumRequest SetOrderDetails(CreateIndiciumRequest request, Shipping.Abstractions.Models.Shipment shipment)
        {
            return request;
        }
    }
}
