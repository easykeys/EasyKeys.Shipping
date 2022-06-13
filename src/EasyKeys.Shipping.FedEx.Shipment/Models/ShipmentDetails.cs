using EasyKeys.Shipping.Abstractions.Models;
using EasyKeys.Shipping.FedEx.Abstractions.Models;

namespace EasyKeys.Shipping.FedEx.Shipment.Models;

public class ShipmentDetails
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
    /// Custom transaction id passed to the request.
    /// </summary>
    public string TransactionId { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Recipient Account Number.
    /// </summary>
    public string AccountNumber { get; set; } = string.Empty;

    /// <summary>
    /// Direct,
    /// Indirect,
    /// Adult,
    /// NoSignature,
    /// Service_Default.
    /// </summary>
    public string DeliverySignatureOptions { get; set; } = "service_default";

    /// <summary>
    /// set email specifications.
    /// </summary>
    public EmailNotification EmailNotification { get; set; } = new EmailNotification();

    /// <summary>
    /// customer_reference,
    /// department_number,
    /// intracountry_regulatory_reference,
    /// invoice_number,
    /// po_number,
    /// rma_association,
    /// shipment_integrity.
    /// </summary>
    public string CustomerReferenceType { get; set; } = "customer_reference";

    /// <summary>
    /// LIST,
    /// NONE,
    /// PREFERRED.
    /// </summary>
    public string RateRequestType { get; set; } = "LIST";

    /// <summary>
    /// Default payment type would be sender unless its a COD.
    /// Sender,ThirdParty,Recipient,Account,Collect.
    /// </summary>
    public FedExPaymentType PaymentType { get; set; } = FedExPaymentType.Sender;

    /// <summary>
    /// Collect on delivery is defaulted to false.
    /// </summary>
    public FedExCollectOnDelivery? CollectOnDelivery { get; set; }

    /// <summary>
    /// FedEx label options.
    /// </summary>
    public LabelOptions LabelOptions { get; set; } = new();
}
