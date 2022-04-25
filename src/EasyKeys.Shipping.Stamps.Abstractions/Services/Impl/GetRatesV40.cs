
using EasyKeys.Shipping.Abstractions.Models;

using StampsClient.v111;

namespace EasyKeys.Shipping.Stamps.Abstractions.Services.Impl
{
    public class GetRatesV40 : IGetRatesV40
    {
        private readonly IStampsClientService _stampsClient;

        public GetRatesV40(IStampsClientService stampsClientService)
        {
            _stampsClient = stampsClientService;
        }

        public async Task<List<RateV40>> GetRatesResponseAsync(Shipment shipment, ShipmentDetails details, Models.ServiceType serviceType, CancellationToken cancellationToken = default)
        {
            var stampsClient = _stampsClient.CreateClient();

            var request = new GetRatesRequest()
            {
                Item = await _stampsClient.GetTokenAsync(cancellationToken),

                Rate = new RateV40()
                {
                    From = new StampsClient.v111.Address()
                    {
                        FullName = details.Sender.FullName,

                        FirstName = details.Sender.FirstName,

                        LastName = details.Sender.LastName,

                        Address1 = shipment.OriginAddress.StreetLine,

                        State = shipment.OriginAddress.StateOrProvince,

                        ZIPCode = shipment.OriginAddress.PostalCode,
                    },

                    To = new StampsClient.v111.Address()
                    {
                        FullName = details.Sender.FullName,

                        FirstName = details.Sender.FirstName,

                        LastName = details.Sender.LastName,

                        Address1 = shipment.DestinationAddress.StreetLine,

                        State = shipment.DestinationAddress.StateOrProvince,

                        PostalCode = shipment.DestinationAddress.PostalCode,

                        ZIPCode = shipment.DestinationAddress.PostalCode,
                    },

                    Amount = 0.0m,

                    MaxAmount = 0.0m,

                    ServiceType = serviceType switch
                    {
                        Models.ServiceType.USPS_PARCEL_SELECT_GROUND => ServiceType.USPS,
                        Models.ServiceType.USPS_FIRST_CLASS_MAIL => ServiceType.USFC,
                        Models.ServiceType.USPS_MEDIA_MAIL => ServiceType.USMM,
                        Models.ServiceType.USPS_PRIORITY_MAIL => ServiceType.USPM,
                        Models.ServiceType.USPS_PRIORITY_MAIL_EXPRESS => ServiceType.USXM,
                        Models.ServiceType.USPS_PRIORITY_MAIL_EXPRESS_INTERNATIONAL => ServiceType.USPMI,
                        Models.ServiceType.USPS_FIRST_CLASS_MAIL_INTERNATIONAL => ServiceType.USFCI,
                        Models.ServiceType.USPS_PAY_ON_USE_RETURN => ServiceType.USRETURN,
                        Models.ServiceType.USPS_LIBRARY_MAIL => ServiceType.USLM,
                        _ => ServiceType.Unknown
                    },

                    ServiceDescription = details.ServiceDescription,

                    PrintLayout = details.LabelOptions.LabelSize,

                    DeliverDays = string.Empty,

                    ShipDate = shipment.Options.ShippingDate,

                    //DeliveryDate =

                    InsuredValue = 100.0m,

                    RegisteredValue = 0.0m,

                    CODValue = details.CollectOnDelivery.Amount,

                    DeclaredValue = 0.0m,

                    NonMachinable = false,

                    RectangularShaped = true,

                    Prohibitions = String.Empty,

                    Restrictions = String.Empty,

                    Observations = String.Empty,

                    Regulations = String.Empty,

                    GEMNotes = String.Empty,

                    MaxDimensions = String.Empty,

                    DimWeighting = String.Empty,

                    //Surcharges =

                    EffectiveWeightInOunces = 0,

                    Zone = 0,

                    RateCategory = 0,

                    CubicPricing = false,

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

                    ContentTypeSpecified = true,

                    // dont know what this is
                    EntryFacility = EntryFacilityV1.Unknown,

                    // dont know what this is
                    SortType = SortTypeV1.Unknown,
                },
                Carrier = details.Carrier.ToLower() switch
                {
                    "usps" => Carrier.USPS,
                    "ups" => Carrier.UPS,
                    "dhlexpress" => Carrier.DHLExpress,
                    "fedex" => Carrier.FedEx,
                    _ => Carrier.USPS
                }
            };

            request = ApplyPackageDetails(request, shipment);

            try
            {
                var response = await stampsClient.GetRatesAsync(request);

                response = ApplyAddOns(response, shipment);

                return response.Rates.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private GetRatesResponse ApplyAddOns(GetRatesResponse request, Shipment shipment)
        {
            var addOns = new List<AddOnV17>();

            foreach (var rate in request.Rates)
            {
                if (rate.RequiresAllOf != null)
                {
                    rate.RequiresAllOf.FirstOrDefault()
                        ?.AsEnumerable()
                        ?.ToList()
                        ?.ForEach(
                            x => addOns.Add(new AddOnV17() { AddOnType = x }));
                }

                addOns.Add(new AddOnV17() { AddOnDescription = "Tracking", AddOnType = AddOnTypeV17.USADC });

                addOns.Add(new AddOnV17() { AddOnDescription = "Registered Mail", AddOnType = AddOnTypeV17.USAREG });

                if (shipment.Packages.Any(x => x.SignatureRequiredOnDelivery))
                {
                    addOns.Add(new AddOnV17() { AddOnDescription = "Signature Confirmation", AddOnType = AddOnTypeV17.USASC });
                }

                rate.AddOns = addOns.ToArray();
            }

            return request;
        }

        private GetRatesRequest ApplyPackageDetails(GetRatesRequest request, Shipment shipment)
        {

            request.Rate.WeightLb = (double)shipment.Packages.Sum(x => x.Weight);

            request.Rate.WeightOz = 0.0;

            request.Rate.PackageType = shipment.Options.PackagingType switch
            {
                _ => PackageTypeV11.Package
            };

            //RequiresAllOf =

            request.Rate.Length = 1.0d;

            request.Rate.Width = 1.0d;

            request.Rate.Height = 1.0d;

            return request;
        }
    }
}
