namespace EasyKeys.Shipping.Abstractions.Models;

public class Rate : ValueObject
{
    public Rate(
        string name,
        string serviceName,
        string packageType,
        decimal totalCharges,
        DateTime? guaranteedDelivery)
        : this(name, serviceName, packageType, totalCharges, 0.0M, guaranteedDelivery, false, "US")
    {
    }

    public Rate(
        string name,
        string serviceName,
        string packageType,
        decimal totalCharges,
        decimal totalCharges2,
        DateTime? guaranteedDelivery) : this(name, serviceName, packageType, totalCharges, totalCharges2, guaranteedDelivery, false, "US")
    {
    }

    public Rate(
        string name,
        string serviceName,
        string packageType,
        decimal totalCharges,
        DateTime? guaranteedDelivery,
        bool saturdayDelivery) : this(name, serviceName, packageType, totalCharges, 0.0M, guaranteedDelivery, saturdayDelivery, "US")
    {
    }

    public Rate(
        string name,
        string serviceName,
        string packageType,
        decimal totalCharges,
        decimal totalCharges2,
        DateTime? guaranteedDelivery,
        bool saturdayDelivery,
        string currencyCode)
    {
        Name = name;
        ServiceName = serviceName;
        PackageType = packageType;
        TotalCharges = totalCharges;
        TotalCharges2 = totalCharges2;
        GuaranteedDelivery = guaranteedDelivery;
        SaturdayDelivery = saturdayDelivery;
        CurrencyCode = currencyCode;
    }

    /// <summary>
    ///     A Name of the rate, as specified by the provider.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Service Provider Name.
    /// </summary>
    public string ServiceName { get; set; }

    /// <summary>
    /// Package Type of shipment.
    /// </summary>
    public string PackageType { get; set; }

    /// <summary>
    ///     The guaranteed date and time of delivery for this rate.
    /// </summary>
    public DateTime? GuaranteedDelivery { get; set; }

    /// <summary>
    /// Saturday Delivery indicator.
    /// </summary>
    public bool SaturdayDelivery { get; set; }

    /// <summary>
    ///     The total cost of this rate.
    /// </summary>
    public decimal TotalCharges { get; set; }

    /// <summary>
    /// In case with FedEx we can have two layers of charges.
    /// </summary>
    public decimal TotalCharges2 { get; set; }

    /// <summary>
    ///     Currency code, if applicable.
    /// </summary>
    public string CurrencyCode { get; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Name;
        yield return ServiceName;
        yield return PackageType;
        yield return GuaranteedDelivery ?? default;
        yield return SaturdayDelivery;
        yield return TotalCharges;
        yield return TotalCharges2;
        yield return CurrencyCode;
    }
}
