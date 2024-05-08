using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

using EasyKeys.Shipping.Abstractions.Extensions;
using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.Usps.Abstractions.Options;

using Microsoft.Extensions.Options;

namespace EasyKeys.Shipping.Usps.Rates;

/// <summary>
/// <see href="https://www.usps.com/business/web-tools-apis/domestic-mail-service-standards-api.htm"/>
/// <see href="https://www.usps.com/business/web-tools-apis/rate-calculator-api_files/rate-calculator-api.htm"/>.
/// </summary>
public class UspsRateProvider : IUspsRateProvider
{
    private readonly HttpClient _httpClient;
    private readonly IOptions<UspsOptions> _options;

    public UspsRateProvider(
        HttpClient httpClient,
        IOptions<UspsOptions> options)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _options = options;
    }

    public async Task<Shipment> GetRatesAsync(
        Shipment shipment,
        UspsRateOptions rateOptions,
        CancellationToken cancellationToken = default)
    {
        shipment = shipment.IsDomestic()
            ? await UspsDomesticRate(shipment, rateOptions).ConfigureAwait(false)
            : await UspsInternationalRate(shipment, rateOptions);

        return shipment;
    }

    private async Task<Shipment> UspsInternationalRate(Shipment shipment, UspsRateOptions rateOptions)
    {
        try
        {
            var sb = CreateInternationalRequest(shipment);

            var rateUri = new Uri(_options.Value.BaseUri, $"?API=IntlRateV2&XML={sb}");
            var response = await _httpClient.GetStringAsync(rateUri).ConfigureAwait(false);

            shipment = ParseInternationalResult(shipment, rateOptions, response);
        }
        catch (Exception ex)
        {
            shipment.InternalErrors.Add($"{nameof(UspsRateProvider)}Domestic exception: {ex?.Message}");
        }

        return shipment;
    }

    private async Task<Shipment> UspsDomesticRate(Shipment shipment, UspsRateOptions rateOptions)
    {
        try
        {
            var sb = CreateDometicRequest(shipment, rateOptions);

            var rateUri = new Uri(_options.Value.BaseUri, $"?API=RateV4&XML={sb}");
            var response = await _httpClient.GetStringAsync(rateUri).ConfigureAwait(false);
            var specialServiceCodes = new List<string>();
            if (shipment.Packages.Any(x => x.SignatureRequiredOnDelivery))
            {
                specialServiceCodes.Add("119"); // 119 represents Adult Signature Requiredgnature Required
            }

            shipment = ParseDomesticResult(shipment, response, rateOptions, specialServiceCodes);
        }
        catch (Exception ex)
        {
            shipment.InternalErrors.Add($"{nameof(UspsRateProvider)}Domestic exception: {ex?.Message}");
        }

        return shipment;
    }

    private string CreateDometicRequest(Shipment shipment, UspsRateOptions rateOptions)
    {
        var sb = new StringBuilder();

        // var signatureOnDeliveryRequired = false;
        var settings = new XmlWriterSettings
        {
            Indent = false,
            OmitXmlDeclaration = true,
            NewLineHandling = NewLineHandling.None
        };

        using (var writer = XmlWriter.Create(sb, settings))
        {
            writer.WriteStartElement("RateV4Request");
            writer.WriteAttributeString("USERID", _options.Value.UserId);
            if (!rateOptions.BaseRatesOnly)
            {
                writer.WriteElementString("Revision", "2");
            }

            var i = 0;
            foreach (var package in shipment.Packages)
            {
                string size;
                var container = shipment?.Options?.PackagingType?.Replace("_", " ");
                if (package.IsPackageLarge())
                {
                    size = "LARGE";

                    // Container must be RECTANGULAR or NONRECTANGULAR when SIZE is LARGE
                    if (container == null || !string.Equals(container, "NONRECTANGULAR", StringComparison.InvariantCultureIgnoreCase))
                    {
                        container = "RECTANGULAR";
                    }
                }
                else
                {
                    size = "REGULAR";
                    if (string.IsNullOrEmpty(container))
                    {
                        container = string.Empty;
                    }
                }

                writer.WriteStartElement("Package");
                writer.WriteAttributeString("ID", i.ToString());
                writer.WriteElementString("Service", rateOptions.ServiceName);
                if (rateOptions.ServiceName.Contains("First"))
                {
                    writer.WriteElementString("FirstClassMailType", container);
                }

                writer.WriteElementString("ZipOrigination", shipment!.OriginAddress.CountryCode == "US" && shipment.OriginAddress.PostalCode.Length > 5 ? shipment.OriginAddress.PostalCode.Substring(0, 5) : shipment.OriginAddress.PostalCode);
                writer.WriteElementString("ZipDestination", shipment.DestinationAddress.CountryCode == "US" && shipment.DestinationAddress.PostalCode.Length > 5 ? shipment.DestinationAddress.PostalCode.Substring(0, 5) : shipment.DestinationAddress.PostalCode);
                writer.WriteElementString("Pounds", package.PoundsAndOunces.Pounds.ToString());
                writer.WriteElementString("Ounces", package.PoundsAndOunces.Ounces.ToString());
                if (rateOptions.ServiceName == "First Class")
                {
                    writer.WriteElementString("Container", string.Empty);
                }
                else
                {
                    writer.WriteElementString("Container", container);
                }

                writer.WriteElementString("Size", size);
                writer.WriteElementString("Width", package.Dimensions.RoundedWidth.ToString());
                writer.WriteElementString("Length", package.Dimensions.RoundedLength.ToString());
                writer.WriteElementString("Height", package.Dimensions.RoundedHeight.ToString());
                writer.WriteElementString("Girth", package.Dimensions.Girth.ToString());
                writer.WriteElementString("Machinable", package.IsPackageMachinable().ToString());
                if (shipment?.Options?.ShippingDate != null)
                {
                    writer.WriteElementString(
                        "ShipDate",
                        shipment.Options.ShippingDate.AddDays(1).ToString("yyyy-MM-dd"));
                }

                if (package.SignatureRequiredOnDelivery)
                {
                    // signatureOnDeliveryRequired = true;
                }

                writer.WriteEndElement();
                i++;
            }

            writer.WriteEndElement();
            writer.Flush();
        }

        return sb.ToString();
    }

    private Shipment ParseDomesticResult(
        Shipment shipment,
        string response,
        UspsRateOptions rateOptions,
        IList<string>? includeSpecialServiceCodes = null)
    {
        var document = XElement.Parse(response, LoadOptions.None);

        var rates = from item in document.Descendants("Postage")
                    group item by (string)item.Element("MailService")!
                    into g
                    select new
                    {
                        Name = g.Key,
                        TotalCharges = g.Sum(x => decimal.Parse((string)x.Element("Rate")!)),
                        DeliveryDate = g.Select(x => (string)x.Element("CommitmentDate")!).FirstOrDefault(),
                        SpecialServices = g.Select(x => x.Element("SpecialServices")).FirstOrDefault()
                    };

        foreach (var r in rates)
        {
            var name = Regex.Replace(r.Name, "&lt.*&gt;", string.Empty);
            var additionalCharges = 0.0m;

            if (includeSpecialServiceCodes?.Count > 0 && r.SpecialServices != null)
            {
                var specialServices = r.SpecialServices.XPathSelectElements("SpecialService").ToList();
                if (specialServices.Count > 0)
                {
                    foreach (var specialService in specialServices)
                    {
                        var serviceId = (string)specialService.Element("ServiceID")!;
                        var price = decimal.Parse((string)specialService.Element("Price")!);

                        if (includeSpecialServiceCodes.Contains(serviceId))
                        {
                            additionalCharges += price;
                        }
                    }
                }
            }

            var charges = r.TotalCharges + additionalCharges;

            if (r.DeliveryDate != null && DateTime.TryParse(r.DeliveryDate, out var deliveryDate))
            {
                var saturdayDelivery = shipment.Options.SaturdayDelivery && deliveryDate.DayOfWeek == DayOfWeek.Saturday;

                shipment.Rates.Add(new Rate(
                    $"USPS {name}",
                    name,
                    shipment.Options.PackagingType,
                    charges,
                    charges,
                    (DateTime?)deliveryDate,
                    saturdayDelivery,
                    shipment.Options.GetCurrencyCode()));
            }
            else
            {
                shipment.Rates.Add(new Rate(
                    $"USPS {name}",
                    name,
                    shipment.Options.PackagingType,
                    charges,
                    charges,
                    rateOptions.DefaultGuaranteedDelivery,
                    false,
                    shipment.Options.GetCurrencyCode()));
            }
        }

        // check for errors
        return document.ParseErrors(shipment);
    }

    private string CreateInternationalRequest(Shipment shipment)
    {
        var sb = new StringBuilder();

        var settings = new XmlWriterSettings
        {
            Indent = false,
            OmitXmlDeclaration = true,
            NewLineHandling = NewLineHandling.None
        };

        using (var writer = XmlWriter.Create(sb, settings))
        {
            writer.WriteStartElement("IntlRateV2Request");
            writer.WriteAttributeString("USERID", _options.Value.UserId);

            writer.WriteElementString("Revision", "2");
            var i = 0;
            foreach (var package in shipment.Packages)
            {
                // <Package ID="2ND">
                //  <Pounds>0</Pounds>
                //  <Ounces>3</Ounces>
                //  <MailType>Envelope</MailType>
                //  <ValueOfContents>750</ValueOfContents>
                //  <Country>Algeria</Country>
                //  <Container></Container>
                //  <Size>REGULAR</Size>
                //  <Width></Width>
                //  <Length></Length>
                //  <Height></Height>
                //  <Girth></Girth>
                //  <CommercialFlag>N</CommercialFlag>
                // </Package>
                var container = shipment.Options.PackagingType?.Replace("_", " ");

                writer.WriteStartElement("Package");
                writer.WriteAttributeString("ID", i.ToString());
                writer.WriteElementString("Pounds", package.PoundsAndOunces.Pounds.ToString());
                writer.WriteElementString("Ounces", package.PoundsAndOunces.Ounces.ToString());
                writer.WriteElementString("MailType", container);
                writer.WriteElementString("ValueOfContents", package.InsuredValue.ToString());
                writer.WriteElementString("Country", shipment.DestinationAddress.GetCountryName());
                writer.WriteElementString("Container", "RECTANGULAR");
                writer.WriteElementString("Size", "REGULAR");
                writer.WriteElementString("Width", package.Dimensions.RoundedWidth.ToString());
                writer.WriteElementString("Length", package.Dimensions.RoundedLength.ToString());
                writer.WriteElementString("Height", package.Dimensions.RoundedHeight.ToString());
                writer.WriteElementString("Girth", package.Dimensions.Girth.ToString());
                writer.WriteElementString("OriginZip", shipment.OriginAddress.PostalCode);
                writer.WriteElementString("CommercialFlag", !shipment.DestinationAddress.IsResidential ? "Y" : "N");

                if (!string.IsNullOrEmpty(shipment.DestinationAddress.PostalCode))
                {
                    writer.WriteElementString("AcceptanceDateTime", shipment.Options.ShippingDate.AddDays(1).ToString("yyyy-MM-ddTHH\\:mm\\:ssZ"));
                    writer.WriteElementString("DestinationPostalCode", shipment.DestinationAddress.PostalCode);
                }

                // TODO: Figure out DIM Weights
                // writer.WriteElementString("Size", package.IsOversize ? "LARGE" : "REGULAR");
                // writer.WriteElementString("Length", package.RoundedLength.ToString());
                // writer.WriteElementString("Width", package.RoundedWidth.ToString());
                // writer.WriteElementString("Height", package.RoundedHeight.ToString());
                // writer.WriteElementString("Girth", package.CalculatedGirth.ToString());
                i++;
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
            writer.Flush();
        }

        return sb.ToString();
    }

    private Shipment ParseInternationalResult(
        Shipment shipment,
        UspsRateOptions rateOptions,
        string response)
    {
        var document = XDocument.Load(new StringReader(response));

        var rates = document
            .Descendants("Service")
            .GroupBy(item => (string)item.Element("SvcDescription")!)
            .Select(g => new
            {
                Name = g.Key,
                DeliveryDate = g.Select(x => (string)x.Element("GuaranteeAvailability")!).FirstOrDefault(),
                TotalCharges = g.Sum(x => decimal.Parse((string)x.Element("Postage")!))
            });

        foreach (var r in rates)
        {
            var name = Regex.Replace(r.Name, "&lt.*gt;", string.Empty);

            if (rateOptions.ServiceName == name
                || rateOptions.ServiceName == "ALL")
            {
                if (r.DeliveryDate != null && DateTime.TryParse(r.DeliveryDate, out var deliveryDate))
                {
                    shipment.Rates.Add(new Rate(
                        $"USPS {name}",
                        name,
                        shipment.Options.PackagingType,
                        r.TotalCharges,
                        r.TotalCharges,
                        (DateTime?)deliveryDate,
                        false,
                        shipment.Options.PreferredCurrencyCode));
                }
                else
                {
                    shipment.Rates.Add(new Rate(
                        $"USPS {name}",
                        name,
                        shipment.Options.PackagingType,
                        r.TotalCharges,
                        r.TotalCharges,
                        rateOptions.DefaultGuaranteedDelivery,
                        false,
                        shipment.Options.PreferredCurrencyCode));
                }
            }
        }

        // check for errors
        return document?.Root?.ParseErrors(shipment) ?? shipment;
    }
}
