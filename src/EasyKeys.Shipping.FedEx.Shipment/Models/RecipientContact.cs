namespace EasyKeys.Shipping.FedEx.Shipment.Models;

public class RecipientContact : SenderContact
{
   public string AccountNumber { get; set; } = string.Empty;

   public bool IsReceipentPaying => !string.IsNullOrEmpty(AccountNumber);
}
