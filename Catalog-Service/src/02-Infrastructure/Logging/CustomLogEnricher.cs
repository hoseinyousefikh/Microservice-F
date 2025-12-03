using Serilog.Core;
using Serilog.Events;

namespace Catalog_Service.src._02_Infrastructure.Logging
{
    public class CustomLogEnricher : ILogEventEnricher
    {
        private readonly string _serviceName;
        private readonly string _serviceVersion;

        public CustomLogEnricher()
        {
            _serviceName = "CatalogService";
            _serviceVersion = "1.0.0";
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("ServiceName", _serviceName));
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("ServiceVersion", _serviceVersion));
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("CorrelationId", CorrelationContextManager.GetCorrelationId()));
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("UserId", CorrelationContextManager.GetUserId()));
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("RequestPath", CorrelationContextManager.GetRequestPath()));
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("IpAddress", CorrelationContextManager.GetIpAddress()));
        }
    }

    public static class CorrelationContextManager
    {
        private static readonly AsyncLocal<CorrelationContextHolder> _current = new AsyncLocal<CorrelationContextHolder>();

        public static CorrelationContext Current
        {
            get
            {
                return _current.Value?.Context;
            }
            set
            {
                var holder = _current.Value;
                if (holder != null)
                {
                    holder.Context = null;
                }

                if (value != null)
                {
                    _current.Value = new CorrelationContextHolder { Context = value };
                }
            }
        }

        public static string GetCorrelationId()
        {
            return Current?.CorrelationId;
        }

        public static string GetUserId()
        {
            return Current?.UserId;
        }

        public static string GetRequestPath()
        {
            return Current?.RequestPath;
        }

        public static string GetIpAddress()
        {
            return Current?.IpAddress;
        }

        private class CorrelationContextHolder
        {
            public CorrelationContext Context;
        }
    }

    public class CorrelationContext
    {
        public string CorrelationId { get; set; }
        public string UserId { get; set; }
        public string RequestPath { get; set; }
        public string IpAddress { get; set; }
        public DateTime StartTime { get; set; } = DateTime.UtcNow;
    }
}

