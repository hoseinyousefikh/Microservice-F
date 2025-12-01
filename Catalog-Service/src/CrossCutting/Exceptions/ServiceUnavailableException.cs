namespace Catalog_Service.src.CrossCutting.Exceptions
{
    public class ServiceUnavailableException : Exception
    {
        public ServiceUnavailableException() : base("The service is currently unavailable.")
        {
        }

        public ServiceUnavailableException(string message) : base(message)
        {
        }

        public ServiceUnavailableException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public ServiceUnavailableException(string serviceName, string reason)
            : base($"Service '{serviceName}' is unavailable. {reason}")
        {
            ServiceName = serviceName;
            Reason = reason;
        }

        public ServiceUnavailableException(string serviceName, string reason, TimeSpan? retryAfter = null)
            : base($"Service '{serviceName}' is unavailable. {reason}")
        {
            ServiceName = serviceName;
            Reason = reason;
            RetryAfter = retryAfter;
        }

        public string ServiceName { get; }
        public string Reason { get; }
        public TimeSpan? RetryAfter { get; }
    }
}
