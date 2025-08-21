using CloudinaryDotNet;
using Efreshli.Application;
using Efreshli.Application.DTOs.CategoryDTOs;
using Efreshli.Application.Services;
using Efreshli.Application.Services.AuthServices;
using Efreshli.Application.Validators.CategoryValidators;
using Efreshli.Common;
using Efreshli.Domain.Common.Classes;
using Efreshli.Domain.Common.Interfaces;
using Efreshli.Domain.Models;
using Efreshli.Domain.Settings;
using Efreshli.Infrastructure;
using Efreshli.Infrastructure.Data;
using Efreshli.Infrastructure.Repositories;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
            builder.Services.AddDbContext<EfreshliDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("sqlConnection")));

            //builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

            builder.Services.AddHttpContextAccessor();

            #region CROS
            var AllowOrogins = "_myAllowSpecificOrigins";

           

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: AllowOrogins,
                                  policy =>
                                  {
                                      policy.WithOrigins("http://localhost:4200/");
                                  });
            });
            #endregion


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





            #region Auth
            //email
            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
            builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromHours(24); // Email confirmation tokens expire in 24 hours
            });

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {

                options.SignIn.RequireConfirmedEmail = true; // This is correct for login
                options.SignIn.RequireConfirmedAccount = true;

                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;
            })
                .AddEntityFrameworkStores<EfreshliDbContext>()
                 .AddDefaultTokenProviders();

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
                    ClockSkew = TimeSpan.Zero,
                    RequireExpirationTime = true,
                    ValidateActor = false,
                    ValidateTokenReplay = false
                };
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        Console.WriteLine($"Token validated for: {context.Principal.Identity.Name}");
                        return Task.CompletedTask;
                    }
                };
            });

            #endregion

            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddApplication(builder.Configuration);


            #region Swagger
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Efreshli API",
                    Version = "v1",
                    Description = "API for Efreshli application"
                });
                // Add security definition for JWT Bearer
                options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    BearerFormat = "JWT",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    Scheme = "Bearer"
                });
                options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                    {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"

                            }
                        },
                        new string[] {}
                    }
                    });

            });


            #endregion

            var app = builder.Build();
            #region localization middleware
            var options = app.Services.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(options.Value);
            #endregion

            var httpContextAccessor = app.Services.GetRequiredService<IHttpContextAccessor>();

            app.UseSwagger();
            app.UseSwaggerUI();
            #region email
            app.UseHttpsRedirection();
            #endregion
            app.UseCors(AllowOrogins);
            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}