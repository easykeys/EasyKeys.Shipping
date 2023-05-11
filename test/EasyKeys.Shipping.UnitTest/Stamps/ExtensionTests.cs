using System.Collections;

using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Abstractions.Extensions;
using EasyKeys.Shipping.Stamps.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Rates.Extensions;
using EasyKeys.Shipping.Stamps.Rates.Models;
using EasyKeys.Shipping.Stamps.Shipment.Extensions;
using EasyKeys.Shipping.Stamps.Shipment.Models;

using EasyKeysShipping.UnitTest.TestHelpers;

using StampsClient.v111;

namespace EasyKeysShipping.UnitTest.Stamps;

public class ExtensionTests
{
    [Fact]
    public void Map_DomesticAddress_Successfully()
    {
        var (_, recipient) = TestShipments.CreateContactInfo();

        var address = TestShipments.CreateDomesticShipment().DestinationAddress;

        var mappedAddress = address.Map(recipient);

        Assert.Equal(mappedAddress.FullName, recipient.FullName);
        Assert.Equal(mappedAddress.FirstName, recipient.FirstName);
        Assert.Equal(mappedAddress.LastName, recipient.LastName);
        Assert.Equal(mappedAddress.PhoneNumber, recipient.PhoneNumber);
        Assert.Equal(mappedAddress.EmailAddress, recipient.Email);

        Assert.Equal(mappedAddress.Address1, address.StreetLine);
        Assert.Equal(mappedAddress.Address2, address.StreetLine2);
        Assert.Equal(mappedAddress.City, address.City);
        Assert.Null(mappedAddress.Province);
        Assert.Equal(mappedAddress.State, address.StateOrProvince);
        Assert.Equal(mappedAddress.Country, address.CountryCode);
        Assert.Null(mappedAddress.PostalCode);
        Assert.Equal(mappedAddress.ZIPCode, address.PostalCode);
    }

    [Fact]
    public void Map_InternationalAddress_Successfully()
    {
        var (_, recipient) = TestShipments.CreateContactInfo();

        var address = TestShipments.CreateInternationalShipment()?.DestinationAddress;

        var mappedAddress = address?.Map(recipient);

        Assert.Equal(mappedAddress?.FullName, recipient.FullName);
        Assert.Equal(mappedAddress?.FirstName, recipient.FirstName);
        Assert.Equal(mappedAddress?.LastName, recipient.LastName);
        Assert.Equal(mappedAddress?.PhoneNumber, recipient.PhoneNumber);
        Assert.Equal(mappedAddress?.EmailAddress, recipient.Email);

        Assert.Equal(mappedAddress?.Address1, address?.StreetLine);
        Assert.Equal(mappedAddress?.Address2, address?.StreetLine2);
        Assert.Equal(mappedAddress?.City, address?.City);
        Assert.Null(mappedAddress?.State);
        Assert.Equal(mappedAddress?.Province, address?.StateOrProvince);
        Assert.Equal(mappedAddress?.Country, address?.CountryCode);
        Assert.Null(mappedAddress?.ZIPCode);
        Assert.Equal(mappedAddress?.PostalCode, address?.PostalCode);
    }

    [Theory]
    [ClassData(typeof(PackageTypeData))]
    public void Map_Shipment_ToRate_Successfully(StampsPackageType packageType)
    {
        var domesticShipment = TestShipments.CreateDomesticShipment();

        var shipment = new Shipment(domesticShipment.OriginAddress, domesticShipment.DestinationAddress, domesticShipment.Packages, new ShipmentOptions(packageType.Name, DateTime.Now));

        var domesticRates = new RateV40().MapToRate(shipment, new EasyKeys.Shipping.Stamps.Rates.Models.RateOptions());

        Assert.NotNull(domesticRates);

        Assert.Equal(domesticRates.PackageType.ToString(), packageType.Name);
    }

    [Theory]
    [ClassData(typeof(ContentTypeData))]
    public void Map_ContentType_Successfully(StampsContentType contentType)
    {
        Assert.Equal(contentType.Value, (int)contentType.Map());
    }

    [Theory]
    [ClassData(typeof(ServiceTypeData))]
    public void Map_RateV40_ServiceType_Successfully(StampsServiceType serviceType)
    {
        var rateOptions = new EasyKeys.Shipping.Stamps.Rates.Models.RateOptions()
        {
            ServiceType = serviceType
        };

        var shipment = TestShipments.CreateDomesticShipment();

        var mappedRate = new RateV40().MapToRate(shipment, rateOptions);

        Assert.NotNull(mappedRate);

        Assert.Equal((int)mappedRate.ServiceType, serviceType.Value);
    }

    [Fact]
    public void Map_RateV40_International_ShipmentSuccessfully()
    {
        var rateOptions = new RateOptions()
        {
            DeclaredValue = 1,
            Observations = "observations",
            Regulations = "regulations",
            GEMNotes = "gemNotes",
            Restrictions = "restrictions",
            Prohibitions = "prohibitions",
            MaxDimensions = "maxDimensions"
        };

        var internationalShipment = TestShipments.CreateInternationalShipment();
        var domesticShipment = TestShipments.CreateDomesticShipment();

        var mappedInternationalRate = new RateV40().MapToRate(internationalShipment!, rateOptions);
        var mappedDomesticRate = new RateV40().MapToRate(domesticShipment, rateOptions);

        Assert.Equal(0, mappedDomesticRate.DeclaredValue);
        Assert.Null(mappedDomesticRate.Observations);
        Assert.Null(mappedDomesticRate.Regulations);
        Assert.Null(mappedDomesticRate.GEMNotes);
        Assert.Null(mappedDomesticRate.Restrictions);
        Assert.Null(mappedDomesticRate.Prohibitions);
        Assert.Null(mappedDomesticRate.MaxDimensions);

        Assert.Equal(mappedInternationalRate.DeclaredValue, rateOptions.DeclaredValue);
        Assert.Equal(mappedInternationalRate.Observations, rateOptions.Observations);
        Assert.Equal(mappedInternationalRate.Regulations, rateOptions.Regulations);
        Assert.Equal(mappedInternationalRate.GEMNotes, rateOptions.GEMNotes);
        Assert.Equal(mappedInternationalRate.Restrictions, rateOptions.Restrictions);
        Assert.Equal(mappedInternationalRate.Prohibitions, rateOptions.Prohibitions);
        Assert.Equal(mappedInternationalRate.MaxDimensions, rateOptions.MaxDimensions);
    }

    [Fact]
    public void Map_ShipmentDetails_Successfully()
    {
        var shipmentDetails = new ShipmentDetails();

        var createIndiciumRequest = new CreateIndiciumRequest().MapToShipmentRequest(
            isDomestic: true,
            weightLb: 0,
            shipmentDetails: new ShipmentDetails(),
            rateOptions: new RateOptions());

        Assert.Equal((int)createIndiciumRequest.PostageMode, shipmentDetails.LabelOptions.PostageMode.Value);
        Assert.Equal((int)createIndiciumRequest.ImageType, shipmentDetails.LabelOptions.ImageType.Value);
        Assert.Equal((int)createIndiciumRequest.EltronPrinterDPIType, shipmentDetails.LabelOptions.DpiType.Value);
        Assert.Equal((int)createIndiciumRequest.PaperSize, shipmentDetails.LabelOptions.PaperSize.Value);
        Assert.Equal((int)createIndiciumRequest.ImageDpi, shipmentDetails.LabelOptions.ImageDPI.Value);
    }

    private class PackageTypeData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { StampsPackageType.Package };

            yield return new object[] { StampsPackageType.LargePackage };

            yield return new object[] { StampsPackageType.PostCard };

            yield return new object[] { StampsPackageType.ThickEnvelope };

            yield return new object[] { StampsPackageType.FlatRateEnvelope };

            yield return new object[] { StampsPackageType.Letter };

            yield return new object[] { StampsPackageType.LargeEnvelopeOrFlat };

            yield return new object[] { StampsPackageType.SmallFlatRateBox };

            yield return new object[] { StampsPackageType.MediumFlatRateBox };

            yield return new object[] { StampsPackageType.LargeFlatRateBox };
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    private class ContentTypeData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { StampsContentType.CommcercialSample };

            yield return new object[] { StampsContentType.DangerousGoods };

            yield return new object[] { StampsContentType.Document };

            yield return new object[] { StampsContentType.Gift };

            yield return new object[] { StampsContentType.HumanitarianDonation };

            yield return new object[] { StampsContentType.Merchandise };

            yield return new object[] { StampsContentType.ReturnedGoods };

            yield return new object[] { StampsContentType.Other };
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    private class ServiceTypeData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { StampsServiceType.ParcelSelectGround };

            yield return new object[] { StampsServiceType.FirstClass };

            yield return new object[] { StampsServiceType.MediaMail };

            yield return new object[] { StampsServiceType.Priority };

            yield return new object[] { StampsServiceType.PriorityExpress };

            yield return new object[] { StampsServiceType.PriorityExpressInternational };

            yield return new object[] { StampsServiceType.FirstClassInternational };

            yield return new object[] { StampsServiceType.LibraryMail };

            yield return new object[] { StampsServiceType.PriorityInternational };

            yield return new object[] { StampsServiceType.Unknown };
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
