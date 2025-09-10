using Efreshli.Application.Interfaces;
using Efreshli.Application.Services.AIServices;
using Efreshli.Application.Services.EmailService;
using Efreshli.Application.Services.ReviewServices;
using Efreshli.Domain.Common.Interfaces;
using Efreshli.Domain.Models;
using Efreshli.Domain.Settings;
using Efreshli.Infrastructure.Data;
using Efreshli.Infrastructure.Repositories;
using Efreshli.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenAI.Chat;

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
            services.AddScoped<IGenericRepository<Cart>, GenericRepository<Cart>>();

            services.AddTransient<IUnitOfWork, UnitOfWork>();

            // Initialize static UserContext
            //UserContext.Initialize(services.BuildServiceProvider());
            //DbInitializer.SeedAsync(services.BuildServiceProvider()).GetAwaiter().GetResult();

            #region External Services
            // FIXED: Email service configuration - use Bind instead
            services.Configure<EmailSettings>(options =>
                configuration.GetSection("EmailSettings").Bind(options));
            services.AddScoped<IEmailService, EmailService>();
            services.AddHttpContextAccessor(); 

            services.AddScoped<IReviewService , ReviewService>();
            #endregion

            //register repositories
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IBrandsRepository, BrandsRepository>();
            services.AddScoped<ICouponRepository, CouponRepository>();
            services.AddScoped<ICartRepository, CartRepository>();

            #region AI dependencies
            services.Configure<OpenAISettings>(configuration.GetSection("OpenAISettings"));

            services.AddSingleton<ChatClient>(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<OpenAISettings>>().Value;

                //var apiKey = string.IsNullOrEmpty(settings.ApiKey)
                //    ? Environment.GetEnvironmentVariable("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")
                //    : settings.ApiKey;
                settings.ApiKey = "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";
                settings.Model = "gpt-4o-mini";
                //if (string.IsNullOrEmpty(apiKey))
                //    throw new InvalidOperationException("OpenAI API key is not configured.");

                return new ChatClient(settings.Model, settings.ApiKey);
            });
            services.AddScoped<IAIService, OpenAIService>();

            #endregion

            return services;
        }
    }
}