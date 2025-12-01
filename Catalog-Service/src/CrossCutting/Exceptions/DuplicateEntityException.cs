namespace Catalog_Service.src.CrossCutting.Exceptions
{
    public class DuplicateEntityException : Exception
    {
        public DuplicateEntityException() : base("A duplicate entity was detected.")
        {
        }

        public DuplicateEntityException(string message) : base(message)
        {
        }

        public DuplicateEntityException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public DuplicateEntityException(string entityType, string identifier)
            : base($"A {entityType} with identifier '{identifier}' already exists.")
        {
            EntityType = entityType;
            Identifier = identifier;
        }

        public string EntityType { get; }
        public string Identifier { get; }
    }
}
