using System;
using System.Collections.Generic;
using System.Text;

using EasyKeys.Shipping.Abstractions.Models;

namespace EasyKeys.Shipping.DHL.Shipment.Models;

public class ShippingDetails
{
    /// <summary>
    /// Sender Contact Info.
    /// </summary>
    public ContactInfo Sender { get; set; } = new ContactInfo();

    /// <summary>
    /// Recipient Contact Info.
    /// </summary>
    public ContactInfo Recipient { get; set; } = new ContactInfo();

    /// <summary>
    /// Required for International Shipments Only.
    /// A collection shipment contents that are considered to be dutiable.
    /// </summary>
    public IList<Commodity> Commodities { get; } = new List<Commodity>();

    /// <summary>
    /// product code returned from rates/products api.
    /// </summary>
    public string ProductCode { get; set; } = string.Empty;

    public List<Logo> Logos { get; set; } = new List<Logo>();

    /// <summary>
    /// signature for the invoice.
    /// </summary>
    public Signature? Signature { get; set; }

    /// <summary>   
    /// key : II insurance, WY paperless.,SF direct signature.
    /// value : amount if required.
    /// </summary>
    public Dictionary<string, double?> AddedServices { get; set; } = new Dictionary<string, double?>();

    public string InvoiceNumber { get; set; } = Guid.NewGuid().ToString();

    public string? CustomShipmentMessage { get; set; } 

    public string? PackageDescription { get; set; }

    public string? LabelDescription { get; set; }

}
