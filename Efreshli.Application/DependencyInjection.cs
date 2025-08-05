using Efreshli.Application.DTOs.CategoryDTOs;
using Efreshli.Application.Helper.Cloudinary;
using Efreshli.Application.Helper.Mapping;
using Efreshli.Application.Interfaces;
using Efreshli.Application.Services.BrandsServices;
using Efreshli.Application.Services.CategoryServices;
using Efreshli.Application.Services.File;
using Efreshli.Application.Validators.CategoryValidators;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            MapsterConfig.RegisterMappings();
            services.AddScoped<ICloudinaryHelper, CloudinaryHelper>();
            services.AddScoped<IImageService, ImageService>();

            // Register validators
            services.AddScoped<IValidator<AddCategoryDto>, AddCategoryDtoValidator>();
            services.AddScoped<IValidator<UpdateCategoryDto>, UpdateCategoryDtoValidator>();

            // Register services
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IBrandsService, BrandsService>();
            return services;
        }
    }
}
