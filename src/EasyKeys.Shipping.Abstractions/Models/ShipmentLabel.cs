namespace EasyKeys.Shipping.Abstractions.Models;

public class ShipmentLabel
{
    /// <summary>
    /// Labels for the <see cref="Package"/>.
    /// </summary>
    public IList<PackageLabelDetails> Labels { get; set; } = new List<PackageLabelDetails>();

    /// <summary>
    ///     Internal library errors during interaction with service provider
    ///     (e.g. SoapException was thrown).
    /// </summary>
    public IList<string> InternalErrors { get; } = new List<string>();
}
