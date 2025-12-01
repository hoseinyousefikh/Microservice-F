namespace Catalog_Service.src.CrossCutting.Exceptions
{
    public class BusinessRuleException : Exception
    {
        public BusinessRuleException() : base("A business rule was violated.")
        {
        }

        public BusinessRuleException(string message) : base(message)
        {
        }

        public BusinessRuleException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public BusinessRuleException(string ruleName, string message)
            : base($"Business rule '{ruleName}' was violated. {message}")
        {
            RuleName = ruleName;
        }

        public string RuleName { get; }
    }
}
