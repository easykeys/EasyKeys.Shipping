namespace EasyKeys.Shipping.Abstractions.Models;
public class ShipmentCancelledResult
{
    public string Message => "Shipment sucessfully cancelled.";

    public List<string> Errors { get; set; } = new List<string>();

    public string FlattenedErrors => string.Join(", ", Errors);

    public bool Succeeded => !Errors.Any();
}
