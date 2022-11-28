using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.Stamps.Abstractions.Models;

namespace EasyKeys.Shipping.Stamps.Shipment.Models;

public class ShipmentDetails
{
    /// <summary>
    /// This is an important value for the label generation.
    /// </summary>
    public StampsPackageType PackageType { get; set; } = StampsPackageType.Package;

    public LabelOptions LabelOptions { get; set; } = new LabelOptions();

    public NotificationOptions NotificationOptions { get; set; } = new NotificationOptions();

    /// <summary>
    /// Required for International Shipments Only.
    /// A collection shipment contents that are considered to be dutiable.
    /// </summary>
    public IList<Commodity> Commodities { get; } = new List<Commodity>();

    /// <summary>
    /// Required for International Shipments Only.
    /// Information required for customs paperwork.
    /// </summary>
    public CustomsInformation CustomsInformation { get; set; } = new CustomsInformation();

    /// <summary>
    /// Generates a sample label. Default is <b>true</b>.
    /// </summary>
    public bool IsSample { get; set; } = true;

    public string OrderId { get; set; } = string.Empty;

    public string IntegratorTxId { get; set; } = Guid.NewGuid().ToString();

    public string CustomerId { get; set; } = Guid.NewGuid().ToString();
}
