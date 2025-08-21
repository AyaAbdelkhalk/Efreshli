using CloudinaryDotNet;
using Efreshli.Application.Interfaces;
using Efreshli.Domain.Common.Classes;
using Efreshli.Domain.Common.Interfaces;
using Efreshli.Domain.Models;
using Efreshli.Infrastructure.Data;
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
            services.AddScoped<IGenericRepository<Image>, GenericRepository<Image>>();

            services.AddScoped<IGenericRepository<Category>, GenericRepository<Category>>();
            services.AddScoped<IGenericRepository<Brand>, GenericRepository<Brand>>();
            services.AddScoped<IGenericRepository<Coupon>, GenericRepository<Coupon>>();
            services.AddScoped<IGenericRepository<Product>, GenericRepository<Product>>();
            services.AddScoped<IProductRepository, ProductRepository>();






            services.AddTransient<IUnitOfWork, UnitOfWork>();
            // Initialize static UserContext
            //UserContext.Initialize(services.BuildServiceProvider());
            DbInitializer.SeedAsync(services.BuildServiceProvider()).GetAwaiter().GetResult();
            


            services.AddHttpContextAccessor();
            //register repositories
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IBrandsRepository, BrandsRepository>();
            services.AddScoped<ICouponRepository, CouponRepository>();


            return services;
        }
    }
}
