using CloudinaryDotNet;
using Efreshli.Application;
using Efreshli.Common;
using Efreshli.Domain.Common.Classes;
using Efreshli.Domain.Models;
using Efreshli.Domain.Settings;
using Efreshli.Infrastructure;
using Efreshli.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace Efreshli.MVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews(options =>
            {
                options.Filters.Add<ValidateModelAsyncFilter>();
            });

            //builder.Services.AddControllers(options =>
            //{
            //    options.Filters.Add<ValidateModelAsyncFilter>();
            //});
            // Database Context
            builder.Services.AddDbContext<EfreshliDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("sqlConnection")));

            #region Cloudinary

            // Cloudinary Configuration
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

            #endregion

            #region Localization
            builder.Services.AddControllersWithViews();
            builder.Services.AddLocalization(opt =>
            {
                opt.ResourcesPath = "";
            });

            builder.Services.AddSingleton<IStringLocalizerFactory, ResourceManagerStringLocalizerFactory>();
            builder.Services.AddScoped(typeof(IStringLocalizer<>), typeof(StringLocalizer<>));

            builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                List<CultureInfo> supportedCultures = new List<CultureInfo>
                {
                    new CultureInfo("en-US"),
                    new CultureInfo("ar-EG")
                };
                options.DefaultRequestCulture = new RequestCulture("ar-EG");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });
            #endregion




            #region RegisterIdentity

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

            #endregion

            // Application and Infrastructure services
            builder.Services.AddApplication(builder.Configuration);
            builder.Services.AddInfrastructure(builder.Configuration);

            var app = builder.Build();
            //// Initialize static UserContext
            //UserContext.Initialize(app.Services);


            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseRouting();
            app.UseStaticFiles();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();


            app.Run();
        }
    }
}
