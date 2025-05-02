using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using VerticalLogistica.Domain.Interfaces;
using VerticalLogistica.Infrastructure.Parsers;
using VerticalLogistica.Infrastructure.Repositories;

namespace VerticalLogistica.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {

            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddSingleton<IOrderParser, OrderParser>();

            return services;
        }
    }
}
