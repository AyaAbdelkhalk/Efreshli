using Efreshli.Application.Interfaces;
using Efreshli.Domain.Common.Interfaces;
using Efreshli.Domain.Models;
using Efreshli.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IGenericRepository<Image>, GenericRepository<Image>>();
            services.AddScoped<ICouponRepository, CouponRepository>();
            return services;
        }
    }
}
