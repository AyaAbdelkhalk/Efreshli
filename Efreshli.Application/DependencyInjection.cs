using Efreshli.Application.DTOs.CategoryDTOs;
using Efreshli.Application.DTOs.CouponDTOs;
using Efreshli.Application.DTOs.WebsiteInfoDTOs;
using Efreshli.Application.Helper.Cloudinary;
using Efreshli.Application.Helper.Mapping;
using Efreshli.Application.Interfaces;
using Efreshli.Application.Services.AddressServices;
using Efreshli.Application.Services.AuthServices;
using Efreshli.Application.Services.BrandsServices;
using Efreshli.Application.Services.CategoryServices;
using Efreshli.Application.Services.ContactUsServices;
using Efreshli.Application.Services.CouponServices;
using Efreshli.Application.Services.File;
using Efreshli.Application.Services.ProductAttributeServices;
using Efreshli.Application.Services.ProductAttributeValueServices;
using Efreshli.Application.Services.ProductItemServices;
using Efreshli.Application.Services.ProductServices;
using Efreshli.Application.Services.RoleService;
using Efreshli.Application.Services.SharedServices;
using Efreshli.Application.Services.UserService;
using Efreshli.Application.Services.WebsiteInfoServices;
using Efreshli.Application.Services.WishlistServices;
using Efreshli.Application.Validators.CategoryValidators;
using Efreshli.Application.Validators.CouponValidators;
using Efreshli.Application.Validators.WebsiteInfoValidators;
using Efreshli.Domain.Common.Classes;
using Efreshli.Domain.Common.Interfaces;
using Efreshli.Domain.Settings;
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
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ICouponService, CouponService>();
            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<IBrandsService, BrandsService>();
            services.AddScoped<IWebsiteInfoService, WebsiteInfoService>(); 
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IProductAttributeService, ProductAttributeService>();
            services.AddScoped<IProductAttributeValueService, ProductAttributeValueService>();
            services.AddScoped<IProductItemService, ProductItemService>();
            services.AddScoped<IWishlistService, WishlistService>();
            services.AddScoped<ISharedService, SharedService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAddressService, AddressService>();
            services.AddScoped<IContactUsService, ContactUsService>();

            // Register external services
            services.AddScoped<ICloudinaryHelper, CloudinaryHelper>();

            // Register all validators from the Application assembly
            //services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(typeof(AddCouponDTOValidator).Assembly);

            // oAuthService
            services.Configure<OAuthSettings>(configuration.GetSection("OAuthSettings"));
            services.AddScoped<IUserContext, UserContext>();
            services.AddHttpClient();



            // Register all validators

            //services.AddScoped<IValidator<CreateWebsiteInfoDto>, CreateWebsiteInfoDtoValidator>(); 
            //services.AddScoped<IValidator<UpdateWebsiteInfoDto>, UpdateWebsiteInfoDtoValidator>(); 


            return services;
        }

    }
}
