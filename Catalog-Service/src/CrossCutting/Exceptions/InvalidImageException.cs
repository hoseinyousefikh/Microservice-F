namespace Catalog_Service.src.CrossCutting.Exceptions
{
    public class InvalidImageException : Exception
    {
        public InvalidImageException() : base("The provided image is invalid.")
        {
        }

        public InvalidImageException(string message) : base(message)
        {
        }

        public InvalidImageException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public InvalidImageException(string fileName, string reason)
            : base($"The image '{fileName}' is invalid. {reason}")
        {
            FileName = fileName;
            Reason = reason;
        }

        public string FileName { get; }
        public string Reason { get; }
    }
}
