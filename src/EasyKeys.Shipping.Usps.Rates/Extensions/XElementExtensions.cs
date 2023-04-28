using System.Xml.Linq;

using EasyKeys.Shipping.Abstractions.Models;

namespace EasyKeys.Shipping.Usps.Rates;

public static class XElementExtensions
{
    public static Shipment ParseErrors(this XElement response, Shipment shipment)
    {
        if (response?.Descendants("Error")?.Any() ?? false)
        {
            var errors = response
                .Descendants("Error")
                .Select(item => new Error()
                {
                    Description = item?.Element("Description")?.Value ?? string.Empty,
                    Source = item?.Element("Source")?.Value ?? string.Empty,
                    HelpContext = item?.Element("HelpContext")?.Value ?? string.Empty,
                    HelpFile = item?.Element("HelpFile")?.Value ?? string.Empty,
                    Number = item?.Element("Number")?.Value ?? string.Empty
                });

            foreach (var err in errors)
            {
                shipment.Errors.Add(err);
            }
        }

        return shipment;
    }
}
