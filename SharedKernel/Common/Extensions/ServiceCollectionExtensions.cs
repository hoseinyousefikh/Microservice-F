using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Application.Abstractions;
using SharedKernel.Application.Abstractions.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.Common.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSharedKernel(this IServiceCollection services)
        {
            //services.AddScoped<IEmailService, EmailService>();
            //services.AddScoped<ISmsService, SmsService>();
            //services.AddScoped<ICacheService, MemoryCacheService>();
            //services.AddScoped<IEventBus, InMemoryEventBus>();

            return services;
        }
    }
}
