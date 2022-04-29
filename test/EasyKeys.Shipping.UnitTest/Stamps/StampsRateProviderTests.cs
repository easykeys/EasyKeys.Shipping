
using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Abstractions.Services;
using EasyKeys.Shipping.Stamps.Rates;

using EasyKeysShipping.UnitTest.TestHelpers;

using Microsoft.Extensions.Logging;

using Moq;

using StampsClient.v111;

namespace EasyKeysShipping.UnitTest.Stamps
{
    public class StampsRateProviderTests
    {
        private readonly ITestOutputHelper _output;

        public StampsRateProviderTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task Return_Shipment_With_Rates_Successfully()
        {
            var rateV40List = new List<RateV40>()
            {
                new RateV40()
                {
                    ServiceType = StampsClient.v111.ServiceType.USFC,
                    ServiceDescription = "TestDescription",
                    Amount = 100m,
                    DeliveryDate = DateTime.Now
                }
            };

            var ratesServiceMock = new Mock<IRatesService>();

            ratesServiceMock.Setup(x => x.GetRatesResponseAsync(
                                It.IsAny<Shipment>(),
                                It.IsAny<RateRequestDetails>(),
                                It.IsAny<CancellationToken>()))
            .ReturnsAsync(rateV40List);

            var iLoggerMock = new Mock<ILogger<StampsRateProvider>>();

            var stampsRateProvider = new StampsRateProvider(
                                         ratesServiceMock.Object,
                                         iLoggerMock.Object);

            var returnedShipment = await stampsRateProvider.GetRatesAsync(
                TestShipments.CreateDomesticShipment(),
                new RateRequestDetails(),
                CancellationToken.None);

            Assert.NotNull(returnedShipment);

            Assert.True(returnedShipment.Rates.All(x => x.Name == StampsClient.v111.ServiceType.USFC.ToString()));

            Assert.True(returnedShipment.Rates.All(x => x.ServiceName == "TestDescription"));

            Assert.True(returnedShipment.Rates.All(x => x.TotalCharges == 100m));

            Assert.True(returnedShipment.Rates.All(x => x.GuaranteedDelivery.GetValueOrDefault().Day == DateTime.Now.Day));
        }

        [Fact]
        public async Task Return_Shipment_With_InternalErrors()
        {
            var rateV40List = new List<RateV40>()
            {
                new RateV40()
                {
                    ServiceType = StampsClient.v111.ServiceType.USFC,
                    ServiceDescription = "TestDescription",
                    Amount = 100m,
                    DeliveryDate = DateTime.Now
                }
            };

            var ratesServiceMock = new Mock<IRatesService>();

            ratesServiceMock.Setup(x => x.GetRatesResponseAsync(
                                It.IsAny<Shipment>(),
                                It.IsAny<RateRequestDetails>(),
                                It.IsAny<CancellationToken>()))
                .Throws(new Exception("Test SOAP Exception"));

            var iLoggerMock = new Mock<ILogger<StampsRateProvider>>();

            var stampsRateProvider = new StampsRateProvider(
                                         ratesServiceMock.Object,
                                         iLoggerMock.Object);

            var returnedShipment = await stampsRateProvider.GetRatesAsync(
                TestShipments.CreateDomesticShipment(),
                new RateRequestDetails(),
                CancellationToken.None);

            Assert.NotNull(returnedShipment);

            Assert.Equal("Test SOAP Exception", returnedShipment.InternalErrors.FirstOrDefault());
        }
    }
}
