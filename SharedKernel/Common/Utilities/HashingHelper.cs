using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.Common.Utilities
{
    public static class HashingHelper
    {
        public static string GenerateHash(string input, string salt)
        {
            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(salt));
            var bytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToBase64String(bytes);
        }

        public static string GenerateSalt()
        {
            using var rng = RandomNumberGenerator.Create();
            var saltBytes = new byte[16];
            rng.GetBytes(saltBytes);
            return Convert.ToBase64String(saltBytes);
        }

        public static bool VerifyHash(string input, string salt, string hash)
        {
            var computedHash = GenerateHash(input, salt);
            return computedHash == hash;
        }
    }
}
