namespace Catalog_Service.src.CrossCutting.Exceptions
{
    public class UnauthorizedAccessException : Exception
    {
        public UnauthorizedAccessException() : base("Access is denied.")
        {
        }

        public UnauthorizedAccessException(string message) : base(message)
        {
        }

        public UnauthorizedAccessException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public UnauthorizedAccessException(string resource, string action)
            : base($"Access denied to '{resource}' for action '{action}'.")
        {
            Resource = resource;
            Action = action;
        }

        public UnauthorizedAccessException(string resource, string action, string userId)
            : base($"User '{userId}' is not authorized to perform action '{action}' on resource '{resource}'.")
        {
            Resource = resource;
            Action = action;
            UserId = userId;
        }

        public string Resource { get; }
        public string Action { get; }
        public string UserId { get; }
    }
}
