using CloudinaryDotNet;
using Efreshli.Application;
using Efreshli.Application.Services.AuthServices;
using Efreshli.Common;
using Efreshli.Domain.Models;
using Efreshli.Domain.Settings;
using Efreshli.Infrastructure;
using Efreshli.Infrastructure.Data;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.Text;


namespace Efreshli.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers(options =>
            {
                options.Filters.Add<ValidateModelAsyncFilter>();
            });
            builder.Services.AddOpenApi();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

      
            builder.Services.AddDbContext<EfreshliDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("sqlConnection")));
            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

            #region Cloudinary
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

          
            builder.Services.AddScoped<IAuthService, AuthService>();

            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddApplication(builder.Configuration);

            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JWT"));

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "JwtBearer";
                options.DefaultChallengeScheme = "JwtBearer";
            }).AddJwtBearer("JwtBearer", options =>
            {
                var jwtSettings = builder.Configuration.GetSection("JWT").Get<JwtSettings>();
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
                    ClockSkew = TimeSpan.Zero
                };
            });


            var app = builder.Build();
            #region localization middleware
            var options = app.Services.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(options.Value);
            #endregion
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseAuthorization();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}