namespace EasyKeys.Shipping.Abstractions.Models
{
    public class LabelDetails
    {
        public Charges Charges { get; set; } = new Charges();

        public string TrackingId { get; set; } = string.Empty;

        public string ImageType { get; set; } = String.Empty;

        public List<byte[]> Bytes { get; set; } = new List<byte[]>();
    }
}
