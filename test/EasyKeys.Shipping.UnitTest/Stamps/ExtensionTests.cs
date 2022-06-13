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
    public ExtensionTests()
    {
    }

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

        Assert.Equal(mappedAddress.FullName, recipient.FullName);
        Assert.Equal(mappedAddress.FirstName, recipient.FirstName);
        Assert.Equal(mappedAddress.LastName, recipient.LastName);
        Assert.Equal(mappedAddress.PhoneNumber, recipient.PhoneNumber);
        Assert.Equal(mappedAddress.EmailAddress, recipient.Email);

        Assert.Equal(mappedAddress.Address1, address.StreetLine);
        Assert.Equal(mappedAddress.Address2, address.StreetLine2);
        Assert.Equal(mappedAddress.City, address.City);
        Assert.Null(mappedAddress.State);
        Assert.Equal(mappedAddress.Province, address.StateOrProvince);
        Assert.Equal(mappedAddress.Country, address.CountryCode);
        Assert.Null(mappedAddress.ZIPCode);
        Assert.Equal(mappedAddress.PostalCode, address.PostalCode);
    }

    [Theory]
    [ClassData(typeof(PackageTypeData))]
    public void Map_Shipment_ToRate_Successfully(StampsPackageType packageType)
    {
        var domesticShipment = TestShipments.CreateDomesticShipment();

        var shipment = new Shipment(domesticShipment.OriginAddress, domesticShipment.DestinationAddress, domesticShipment.Packages, new ShipmentOptions(packageType.Name, DateTime.Now));

        var domesticRates = shipment.MapToRate();

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
        var rateOptions = new RateInternationalOptions()
        {
            ServiceType = serviceType,
            DeclaredValue = 1,
            Observations = "observations",
            Regulations = "regulations",
            GEMNotes = "gemNotes",
            Restrictions = "restrictions",
            Prohibitions = "prohibitions",
            MaxDimensions = "maxDimensions"
        };

        var shipment = TestShipments.CreateDomesticShipment();

        var mappedRate = new RateV40().MapToInternationalRate(shipment, rateOptions);

        Assert.NotNull(mappedRate);

        Assert.Equal((int)mappedRate.ServiceType, serviceType.Value);
        Assert.Equal(mappedRate.DeclaredValue, rateOptions.DeclaredValue);
        Assert.Equal(mappedRate.Observations, rateOptions.Observations);
        Assert.Equal(mappedRate.Regulations, rateOptions.Regulations);
        Assert.Equal(mappedRate.GEMNotes, rateOptions.GEMNotes);
        Assert.Equal(mappedRate.Restrictions, rateOptions.Restrictions);
        Assert.Equal(mappedRate.Prohibitions, rateOptions.Prohibitions);
        Assert.Equal(mappedRate.MaxDimensions, rateOptions.MaxDimensions);
    }

    [Fact]
    public void Map_ShipmentDetails_Successfully()
    {
        var shipmentDetails = new ShipmentDetails();

        var createIndiciumRequest = shipmentDetails.Map();

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
            yield return new object[] { StampsPackageType.Pak };

            yield return new object[] { StampsPackageType.Package };

            yield return new object[] { StampsPackageType.OversizedPackage };

            yield return new object[] { StampsPackageType.LargePackage };

            yield return new object[] { StampsPackageType.PostCard };

            yield return new object[] { StampsPackageType.Documents };

            yield return new object[] { StampsPackageType.ThickEnvelope };

            yield return new object[] { StampsPackageType.Envelope };

            yield return new object[] { StampsPackageType.ExpressEnvelope };

            yield return new object[] { StampsPackageType.FlatRateEnvelope };

            yield return new object[] { StampsPackageType.LegalFlatRateEnvelope };

            yield return new object[] { StampsPackageType.Letter };

            yield return new object[] { StampsPackageType.LargeEnvelopeOrFlat };

            yield return new object[] { StampsPackageType.SmallFlatRateBox };

            yield return new object[] { StampsPackageType.FlatRateBox };

            yield return new object[] { StampsPackageType.LargeFlatRateBox };

            yield return new object[] { StampsPackageType.FlatRatePaddedEnvelope };

            yield return new object[] { StampsPackageType.RegionalRateBoxA };

            yield return new object[] { StampsPackageType.RegionalRateBoxB };

            yield return new object[] { StampsPackageType.RegionalRateBoxC };
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
