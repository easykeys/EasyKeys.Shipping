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
                    Description = item.Element("Description").Value,
                    Source = item.Element("Source").Value,
                    HelpContext = item.Element("HelpContext").Value,
                    HelpFile = item.Element("HelpFile").Value,
                    Number = item.Element("Number").Value
                });

            foreach (var err in errors)
            {
                shipment.Errors.Add(err);
            }
        }

        return shipment;
    }
}
