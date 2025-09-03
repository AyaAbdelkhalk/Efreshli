using CloudinaryDotNet;
using Efreshli.Application;
using Efreshli.Application.DTOs.CouponDTOs;  // This is crucial
using Efreshli.Application.Validators.CouponValidators;
using Efreshli.Common;
using Efreshli.Domain.Common.Classes;
using Efreshli.Domain.Enums;
using Efreshli.Domain.Models;
using Efreshli.Domain.Settings;
using Efreshli.Infrastructure;
using Efreshli.Infrastructure.Data;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Stripe;
using System.Globalization;

namespace Efreshli.MVC
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
          
            builder.Services.AddValidatorsFromAssemblyContaining<AddCouponDTO>(ServiceLifetime.Scoped);
            //builder.Services.AddScoped<IValidator<AddCouponDTO>, AddCouponDTOValidator>();

            //builder.Services.AddControllersWithViews();

            // 1. MVC and Razor support
            //builder.Services.AddControllersWithViews();
            builder.Services.AddControllersWithViews(options =>
            {
                options.Filters.Add<ValidateModelAsyncFilter>();
            });
            builder.Services.AddRazorPages();

            // 2. Database
            builder.Services.AddDbContext<EfreshliDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("sqlConnection"),s=>s.EnableRetryOnFailure()));

            // 3. Identity
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 8;
            })
            .AddEntityFrameworkStores<EfreshliDbContext>()
            .AddDefaultTokenProviders();
            // 3.1 Authorization Policies 
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy(UserRoles.Admin, policy =>
                    policy.RequireRole(UserRoles.Admin));
            });



            // 4. JWT Settings (optional for MVC, but keep if used)
            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JWT"));

            // 5. Application & Infrastructure Layers
            builder.Services.AddApplication(builder.Configuration);
            builder.Services.AddInfrastructure(builder.Configuration);

            // 6. HttpContext
            builder.Services.AddHttpContextAccessor();

            //// 7. Localization
            //builder.Services.AddLocalization(opt =>
            //{
            //    opt.ResourcesPath = "";
            //});

            //builder.Services.Configure<RequestLocalizationOptions>(options =>
            //{
            //    var supportedCultures = new List<CultureInfo>
            //    {
            //        new CultureInfo("en-US"),
            //        new CultureInfo("ar-EG")
            //    };
            //    options.DefaultRequestCulture = new RequestCulture("ar-EG");
            //    options.SupportedCultures = supportedCultures;
            //    options.SupportedUICultures = supportedCultures;
            //});


            // 7. Localization
            builder.Services.AddLocalization();
            builder.Services.AddSingleton<IStringLocalizerFactory, ResourceManagerStringLocalizerFactory>();

            builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new List<CultureInfo>
                    {
                        new CultureInfo("en-US"),
                        new CultureInfo("ar-EG")
                    };

                options.DefaultRequestCulture = new RequestCulture("ar-EG");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;

                // Add cookie provider
                options.AddInitialRequestCultureProvider(new CookieRequestCultureProvider()
                {
                    CookieName = "LanguagePreference",
                    // Optional: if you want to fall back to other providers when cookie is not present
                    Options = options
                });
            });

            // 8. Cloudinary
            builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));

            builder.Services.AddSingleton(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<CloudinarySettings>>().Value;

                var account = new CloudinaryDotNet.Account(
                    settings.CloudName,
                    settings.ApiKey,
                    settings.ApiSecret
                );

                return new Cloudinary(account);
            });


            #region Stripe
            StripeConfiguration.ApiKey = builder.Configuration.GetSection("StripeSettings:Secretkey").Get<string>();
            #endregion

            var app = builder.Build();
            //using (var scope = app.Services.CreateScope())
            //{
            //    var services = scope.ServiceProvider;
            //    var context = services.GetRequiredService<EfreshliDbContext>();
            //    await context.Database.MigrateAsync(); // Apply migrations
            //    await DbInitializer.SeedAsync(services);
            //}

            // Initialize static UserContext
            //UserContext.Initialize(app.Services);


            // Error handling
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            var locOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(locOptions.Value);
            app.Use(async (context, next) =>
            {
                var culture = CultureInfo.CurrentUICulture;
                Console.WriteLine($"Current Culture: {culture}"); // Debug check
                await next();
            });
            app.UseRouting();

            // Auth middlewares if needed
            app.UseAuthentication();
            app.UseAuthorization();

            // MVC routes
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Login}");


            app.Run();
        }
    }
}
