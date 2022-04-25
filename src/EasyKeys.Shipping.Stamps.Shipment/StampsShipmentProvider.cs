using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Abstractions.Services;

using StampsClient.v111;

namespace EasyKeys.Shipping.Stamps.Shipment
{
    public class StampsShipmentProvider : IStampsShipmentProvider
    {
        private readonly IStampsClientService _stampsClient;

        public StampsShipmentProvider(IStampsClientService stampsClientService)
        {
            _stampsClient = stampsClientService;
        }

        public async Task<ShipmentLabel> CreateShipmentAsync(Shipping.Abstractions.Models.Shipment shipment, RateV40 rate, CancellationToken cancellationToken)
        {
            var client = _stampsClient.CreateClient();

            var request = new CreateIndiciumRequest()
            {
                Item = await _stampsClient.GetTokenAsync(cancellationToken),

                IntegratorTxID = Guid.NewGuid().ToString(),

                Rate = rate,

                ReturnTo = rate.From,

                CustomerID = Guid.NewGuid().ToString(),

                // for testing purposes only.
                SampleOnly = true,

                /*Normal	A regular label with postage and a valid indicium.
                 NoPostage	A regular label without postage or an indicium.
                */
                PostageMode = PostageMode.Normal,

                // set image type
                ImageType = ImageType.Png,

                // Resolution of shipping label for thermal printer.
                EltronPrinterDPIType = EltronPrinterDPIType.Default,

                // The memo to print at the bottom of the shipping label. The memo parameter may consist of more than one line separated by the standard carriage return/line feed, use &#xd; as carriage return and &#xa; as line feed in the request
                memo = String.Empty,

                // ??
                cost_code_id = 0,

                // Set to true to send a delivery notification email back to the email address associated with the account when the shipment is delivered.
                deliveryNotification = false,

                // Set to 0, 90, 180, or 270 to represent the number of degrees of counter-clockwise rotation for the label.
                rotationDegrees = 0,

                // Indicates how many units the label should be offset on the x-axis. Applies only to thermal printers
                horizontalOffset = 0,

                // Indicates how many units the label should be offset on the y-axis. Applies only to thermal printers.
                verticalOffset = 0,

                // Density settings for thermal printers, to help control the darkness of the print.
                printDensity = 0,

                // Indicates whether the memo is printed on the label. Set to true to print the memo.
                printMemo = false,

                /* This applies to domestic and international, combined and separate CP72 and CN22 layouts.
                 * For other label types this parameter can be specified but is ignored.
                 * If the parameter is omitted, or is set to “true”, instructions will be included as part of the label.
                */
                printInstructions = false,

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

                /*Determines whether the image URL or the actual image data will be returned in the response object.
                 * If ReturnImageData is false, one or more image URL will be returned in the URL element in the response object.
                 * Each URL will need to be queried to retrieve the actual image.
                 * If ReturnImageData is true, the actual image data will be returned as a base64 string in the ImageData object.
                 * The URL element will be an empty string.
                 * This mode cannot be used on ImageType of Auto, PrintOncePDF, or EncryptedPngURL.
                 */
                ReturnImageData = true,

                /* For International mail, shipments valued over $2500 generally require an
                    Internal Transaction Number (ITN) to be put on the CP72 form.*/
                InternalTransactionNumber = "123",

                /* Specifies the page size of PDF labels. This value only applies to PDF. If offset is specified, this value will be ignored.
                    Default
                    Letter85x11
                    LabelSize
                */
                PaperSize = PaperSizeV1.Default,

                // ??
                PayOnPrint = false,

                // ??
                ReturnLabelExpirationDays = 1,

                /* Specifies the DPI setting for the label image generated for the following ImageType values:
                 * Gif, Jpg, Png. It applies to both the label images in the ImageData and the URL response.*/
                ImageDpi = ImageDpi.ImageDpiDefault,

                // ??
                RateToken = string.Empty,

                // Caller defined data. Order ID associated to this print.
                OrderId = string.Empty,

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

            request = SetShipmentNotification(request, shipment);

            request = SetOrderDetails(request, shipment);

            request = shipment.SenderInformation.Email.Any() ? SetEmailLabelTo(request, shipment) : request;

            request = shipment.Commodities.Any() ? SetCustomsInformation(request, shipment) : request;



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
                            Bytes = response.ImageData.ToList()
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

        /// <summary>
        /// Customs information. Required for International.Default is false.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="shipment"></param>
        /// <returns></returns>
        private CreateIndiciumRequest SetCustomsInformation(CreateIndiciumRequest request, Shipping.Abstractions.Models.Shipment shipment)
        {
            // using shipment.commodities information set request.customs
            return request;
        }

        private CreateIndiciumRequest SetShipmentNotification(CreateIndiciumRequest request, Shipping.Abstractions.Models.Shipment shipment)
        {
            // using shipment informtion to set request.shipmentNotification

            return request;
        }

        private CreateIndiciumRequest SetEmailLabelTo(CreateIndiciumRequest request, Shipping.Abstractions.Models.Shipment shipment)
        {

            //EmailLabelTo = new LabelRecipientInfo()
            return request;
        }

        private CreateIndiciumRequest SetOrderDetails(CreateIndiciumRequest request, Shipping.Abstractions.Models.Shipment shipment)
        {
            //request.OrderDetails
            return request;
        }
    }
}
