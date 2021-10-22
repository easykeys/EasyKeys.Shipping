namespace EasyKeys.Shipping.Stamps.Abstractions.Options
{
    public class StampsOptions
    {
        public bool IsProduction{ get; set; }

        public string Url => IsProduction ? "https://swsim.stamps.com/swsim/swsimv111.asmx" : "https://swsim.testing.stamps.com/swsim/swsimv111.asmx";

        public string IntegrationId { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}
