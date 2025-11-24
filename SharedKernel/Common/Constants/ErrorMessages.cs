using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.Common.Constants
{
    public static class ErrorMessages
    {
        public const string NotFound = "Resource not found.";
        public const string ValidationFailed = "Validation failed.";
        public const string Unauthorized = "Unauthorized access.";
        public const string DuplicateEmail = "Email already exists.";
        public const string InvalidCredentials = "Invalid credentials.";
        public const string InvalidToken = "Invalid or expired token.";
    }
}
