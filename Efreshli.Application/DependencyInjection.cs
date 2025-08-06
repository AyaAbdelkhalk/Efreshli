using Efreshli.Application.DTOs.CategoryDTOs;
using Efreshli.Application.Helper;
using Efreshli.Application.Helper.Cloudinary;
using Efreshli.Application.Interfaces;
using Efreshli.Application.Services.CategoryServices;
using Efreshli.Application.Services.CouponServices;
using Efreshli.Application.Services.File;
using Efreshli.Application.Validators.CategoryValidators;
using Efreshli.Application.Validators.CouponValidators;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            // Register Mapster mappings
            MapsterConfig.RegisterMappings();

            // Register services
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ICouponService, CouponService>();
            services.AddScoped<IImageService, ImageService>();

            // Register external services
            services.AddScoped<ICloudinaryHelper, CloudinaryHelper>();
            // Register all validators from the Application assembly
            //services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(typeof(AddCouponValidator).Assembly);

            return services;
        }
    }
}
