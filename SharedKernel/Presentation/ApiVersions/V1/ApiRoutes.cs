using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.Presentation.ApiVersions.V1
{
    public static class ApiRoutes
    {
        public const string Base = "api/v1";

        public static class Products
        {
            public const string Get = $"{Base}/products";
            public const string GetById = $"{Base}/products/{{id}}";
            public const string Create = $"{Base}/products";
            public const string Update = $"{Base}/products/{{id}}";
            public const string Delete = $"{Base}/products/{{id}}";
        }

        public static class Users
        {
            public const string Get = $"{Base}/users";
            public const string GetById = $"{Base}/users/{{id}}";
            public const string Create = $"{Base}/users";
            public const string Update = $"{Base}/users/{{id}}";
            public const string Delete = $"{Base}/users/{{id}}";
        }

        public static class Auth
        {
            public const string Login = $"{Base}/auth/login";
            public const string Register = $"{Base}/auth/register";
            public const string Refresh = $"{Base}/auth/refresh";
            public const string Logout = $"{Base}/auth/logout";
        }
    }
}
