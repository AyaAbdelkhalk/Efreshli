using Efreshli.Application.Interfaces;
using Efreshli.Application.Services.EmailService;
using Efreshli.Domain.Common.Interfaces;
using Efreshli.Domain.Models;
using Efreshli.Domain.Settings;
using Efreshli.Infrastructure.Data;
using Efreshli.Infrastructure.Repositories;
using Efreshli.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

            services.AddTransient<IUnitOfWork, UnitOfWork>();

            // Initialize static UserContext
            //UserContext.Initialize(services.BuildServiceProvider());
            DbInitializer.SeedAsync(services.BuildServiceProvider()).GetAwaiter().GetResult();

            // FIXED: Email service configuration - use Bind instead
            services.Configure<EmailSettings>(options =>
                configuration.GetSection("EmailSettings").Bind(options));
            services.AddScoped<IEmailService, EmailService>();
            services.AddHttpContextAccessor();

            //register repositories
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IBrandsRepository, BrandsRepository>();
            services.AddScoped<ICouponRepository, CouponRepository>();

            return services;
        }
    }
}