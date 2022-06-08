using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Rates.Extensions;
using EasyKeys.Shipping.Stamps.Shipment.Models;

using StampsClient.v111;

namespace EasyKeys.Shipping.Stamps.Shipment.Extensions;

public static class CreateIndiciumRequestExtensions
{
    public static CreateIndiciumRequest SetShipmentNotification(
        this CreateIndiciumRequest request,
        ShipmentDetails shipmentDetails,
        ContactInfo recipient)
    {
        request.ShipmentNotification = new ShipmentNotification()
        {
            Email = string.IsNullOrEmpty(shipmentDetails.NotificationOptions.Email) ? recipient.Email : shipmentDetails.NotificationOptions.Email,
            CCToAccountHolder = shipmentDetails.NotificationOptions.CC_ToAccountHolder,
            UseCompanyNameInFromLine = shipmentDetails.NotificationOptions.UseCompanyNameInFromLine,
            UseCompanyNameInSubject = shipmentDetails.NotificationOptions.UseCompanyNameInSubject,
        };

        return request;
    }

    public static CreateIndiciumRequest SetCustomsInformation(
        this CreateIndiciumRequest request,
        ContentType contentType,
        IList<Commodity> commodities,
        CustomsInformation customsInformation,
        double weightLb)
    {
        var customsLine = new List<CustomsLine>();

        foreach (var commodity in commodities)
        {
            customsLine.Add(new CustomsLine
            {
                Description = commodity.Description,
                CountryOfOrigin = commodity.CountryOfManufacturer,
                Quantity = commodity.Quantity,
                HSTariffNumber = string.Empty,
                sku = commodity.PartNumber,
                Value = commodity.CustomsValue,
                WeightLb = weightLb,
                WeightOz = 0.0d
            });
        }

        request.Customs = new CustomsV7()
        {
            ContentType = contentType.Map(),

            Comments = commodities?.FirstOrDefault()?.Comments,

            LicenseNumber = commodities?.FirstOrDefault()?.ExportLicenseNumber,

            CertificateNumber = customsInformation.CertificateNumber,

            InvoiceNumber = customsInformation.InvoiceNumber,

            OtherDescribe = customsInformation.OtherDescribe,

            CustomsLines = customsLine.ToArray(),

            CustomsSigner = customsInformation.CustomsSigner,

            PassportNumber = customsInformation.PassportNumber,

            PassportIssueDate = customsInformation.PassportIsseDate,

            PassportExpiryDate = customsInformation.PassportExpiryDate,

            ImportLicenseNumber = customsInformation.ImportLicenseNumber,

            SendersCustomsReference = customsInformation.SendersCustomsReference
        };

        return request;
    }
}
