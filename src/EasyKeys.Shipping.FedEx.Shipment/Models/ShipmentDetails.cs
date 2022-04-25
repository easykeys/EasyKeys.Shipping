namespace EasyKeys.Shipping.FedEx.Shipment.Models;

public class ShipmentDetails
{
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
