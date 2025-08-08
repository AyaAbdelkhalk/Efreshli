//using Efreshli.Domain.Common.Classes;
//using Efreshli.Infrastructure.Data;
//using Microsoft.EntityFrameworkCore;

//namespace Efreshli.MVC
//{
//    public class Program
//    {
//        public static void Main(string[] args)
//        {
//            var builder = WebApplication.CreateBuilder(args);

//            // Add services to the container.
//            builder.Services.AddControllersWithViews();
//            builder.Services.AddDbContext<EfreshliDbContext>(options =>
//                options.UseSqlServer(builder.Configuration.GetConnectionString("sqlConnection")));

//            #region RegisterIdentity


//            #endregion
//            var app = builder.Build();
//            // Initialize static UserContext
//            UserContext.Initialize(app.Services);

//            // Configure the HTTP request pipeline.
//            if (!app.Environment.IsDevelopment())
//            {
//                app.UseExceptionHandler("/Home/Error");
//            }
//            app.UseRouting();
//            app.UseStaticFiles();

//            app.UseAuthorization();

//            app.MapStaticAssets();
//            app.MapControllerRoute(
//                name: "default",
//                pattern: "{controller=Home}/{action=Index}/{id?}")
//                .WithStaticAssets();

//            app.Run();
//        }
//    }
//}
using CloudinaryDotNet;
using Efreshli.Application;

using Efreshli.Domain.Common.Classes;
using Efreshli.Domain.Models;
using Efreshli.Domain.Settings;
using Efreshli.Infrastructure;
using Efreshli.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace Efreshli.MVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. MVC and Razor support
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();

            // 2. Database
            builder.Services.AddDbContext<EfreshliDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("sqlConnection")));

            // 3. Identity
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 3;
            })
            .AddEntityFrameworkStores<EfreshliDbContext>()
            .AddDefaultTokenProviders();

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
            builder.Services.AddLocalization(opt =>
            {
                opt.ResourcesPath = "";
            });

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

                var account = new Account(
                    settings.CloudName,
                    settings.ApiKey,
                    settings.ApiSecret
                );

                return new Cloudinary(account);
            });

            var app = builder.Build();

            // Initialize static UserContext
            UserContext.Initialize(app.Services);

            // Localization middleware
            var locOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(locOptions.Value);

            // Error handling
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseRouting();

            // Auth middlewares if needed
            app.UseAuthentication();
            app.UseAuthorization();

            // MVC routes
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
