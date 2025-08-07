using Efreshli.Domain.Common.Classes;
using Efreshli.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Efreshli.MVC.Data;
using Efreshli.Application;
using Efreshli.Infrastructure;
using CloudinaryDotNet;
using Efreshli.Domain.Settings;
using Microsoft.Extensions.Options;

namespace Efreshli.MVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Database Context
            builder.Services.AddDbContext<EfreshliDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("sqlConnection")));

            // Application and Infrastructure services
            builder.Services.AddApplication(builder.Configuration);
            builder.Services.AddInfrastructure(builder.Configuration);

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

            #region RegisterIdentity


            #endregion
            var app = builder.Build();
            // Initialize static UserContext
            UserContext.Initialize(app.Services);


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
