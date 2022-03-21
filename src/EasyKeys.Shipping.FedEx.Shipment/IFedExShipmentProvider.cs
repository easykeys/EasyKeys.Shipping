namespace EasyKeys.Shipping.FedEx.Shipment
{
    public interface IFedExShipmentProvider
    {/// <summary>
     /// Processes fedex shipping and returns a shipping label.
     /// </summary>
     /// <param name="shipment"></param>
     /// <param name="labelOptions"></param>
     /// <param name="cancellationToken"></param>
     /// <returns></returns>
        Task<Label> ProcessShipmentAsync(
            Shipping.Abstractions.Models.Shipment shipment,
            LabelOptions labelOptions,
            CancellationToken cancellationToken = default);
    }
}
