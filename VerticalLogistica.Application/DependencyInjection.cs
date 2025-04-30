using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using VerticalLogistica.Application.Services;
using VerticalLogistica.Domain.Interfaces;

namespace VerticalLogistica.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Register application services
            services.AddScoped<IOrderService, OrderService>();

            return services;
        }
    }
}
