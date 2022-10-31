namespace EasyKeys.Shipping.Usps.Tracking
{
    public class UspsTrackingClientException : ApplicationException
    {
        public UspsTrackingClientException(string ErrorMessage)
        {
            _Exception = new Exception(ErrorMessage);
        }

        public UspsTrackingClientException(string ErrorMessage, Exception ex)
        {
            _Exception = new Exception(ErrorMessage, ex);
        }

        public UspsTrackingClientException(Exception ex)
        {
            _Exception = ex;
        }

        private readonly Exception _Exception;

        public override string Message => _Exception.Message;

        public override string? Source
        {
            get => _Exception?.Source;
            set => _Exception.Source = value;
        }

        public override string? StackTrace => _Exception?.StackTrace;

        public override System.Collections.IDictionary Data => _Exception.Data;

        public override string? HelpLink
        {
            get => _Exception?.HelpLink;
            set => _Exception.HelpLink = value;
        }
    }
}
