namespace API_Gateway.Configuration
{
    public class AppSettings
    {
        public JwtSettings JwtSettings { get; set; }
        public DownstreamServices DownstreamServices { get; set; }
    }

    public class JwtSettings
    {
        public string SecretKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int ExpirationInMinutes { get; set; }
    }

    public class DownstreamServices
    {
        public string IdentityService { get; set; }
        // سایر سرویس‌ها را اینجا اضافه کنید
    }
}
