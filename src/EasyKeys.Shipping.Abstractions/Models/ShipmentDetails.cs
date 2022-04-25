namespace EasyKeys.Shipping.Abstractions.Models;

public class ShipmentDetails
{
    /// <summary>
    /// A plain language description of the service returned, i.e. “USPS Priority Mail”.
    /// </summary>
    public string ServiceDescription { get; set; } = string.Empty;

    /// <summary>
    /// <list type="bullet">
    /// <item>
    /// <description>
    /// USPS
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// FedEx
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// UPS
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// DHLExpress
    /// </description>
    /// </item>
    /// </list>
    /// </summary>
    public string Carrier { get; set; } = "USPS";

    /// <summary>
    /// <list type="bullet">
    ///    <listheader>
    ///    <term>Fedex Options</term>
    ///    <description>description</description>
    ///    <see href="https://developer.stamps.com/soap-api/reference/swsimv111.html#getratesresponse-object">more documentation</see>
    ///     </listheader>
    /// <item>
    /// <description>default = Merchandise</description>
    /// </item>
    /// <item>
    /// <description>Commercial_Sample</description>
    /// </item>
    /// <item>
    /// <description>Dangerous_Goods</description>
    /// </item>
    /// <item>
    /// <description>Document</description>
    /// </item>
    /// <item>
    /// <description>Gift</description>
    /// </item>
    /// <item>
    /// <description>Humanitarian_Donation</description>
    /// </item>
    /// <item>
    /// <description>Returned_Goods</description>
    /// </item>
    /// <item>
    /// <description>Other</description>
    /// </item>
    /// </list>
    /// One of: Commercial_Sample, Dangerous_Goods, Document, Gift, Humanitarian_Donation, Merchandise, Returned_Goods, or Other.
    /// </summary>
    public string ContentType { get; set; } = "Merchandise";

    /// <summary>
    /// Custom transaction id passed to the request.
    /// </summary>
    public string TransactionId { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Informaiton used for labels.
    /// </summary>
    public SenderContact Sender { get; set; } = new();

    /// <summary>
    /// Information used for labels.
    /// </summary>
    public RecipientContact Recipient { get; set; } = new();

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
    public string RateRequestType { get; set; } = "PREFERRED";

    /// <summary>
    /// Default payment type would be sender unless its a COD.
    /// Sender,ThirdParty,Recipient,Account,Collect.
    /// </summary>
    public string PaymentType { get; set; } = "SENDER";

    /// <summary>
    /// Collect on delivery is defaulted to false.
    /// </summary>
    public CollectOnDelivery CollectOnDelivery { get; set; } = new CollectOnDelivery();

    public LabelOptions LabelOptions { get; set; } = new();
}
