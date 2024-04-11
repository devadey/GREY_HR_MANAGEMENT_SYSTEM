using Application.AppConfigs;
using Common.Responses.Wrappers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using WebApi.Permissions;

namespace WebApi;

public static class ServiceCollectionExtensions
{
    public static IApplicationBuilder SeedDatabase(this IApplicationBuilder app)
    {
        var scope = app.ApplicationServices.CreateScope();
        var seeders = scope.ServiceProvider.GetServices<ApplicationDbSeeder>();

        foreach (var seeder in seeders)
        {
            seeder.SeedDataAsync().GetAwaiter().GetResult();
        }

        return app;
    }

    internal static IServiceCollection AddIdentitySettings(this IServiceCollection services)
    {
        services
            .AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>()
            .AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>()
            .AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.User.RequireUniqueEmail = true;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        return services;
    }


    internal static IServiceCollection AddJWTAuthentication(this IServiceCollection services, AppConfiguration configuration)
    {
        var key = Encoding.ASCII.GetBytes(configuration.Secret);
        services
            .AddAuthentication(Authentication =>
            {
                Authentication.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                Authentication.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(bearer =>
            {
                bearer.RequireHttpsMetadata = false;
                bearer.SaveToken = true;
                bearer.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    RoleClaimType = ClaimTypes.Role,
                    ClockSkew = TimeSpan.Zero
                };

                bearer.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = c =>
                    {
                        if (c.Exception is SecurityTokenExpiredException)
                        {
                            c.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                            c.Response.ContentType = "application/json";
                            var result = JsonConvert.SerializeObject(ResponseWrapper.Fail("The Token is expired."));
                            return c.Response.WriteAsync(result);
                        }
                        else
                        {
                            c.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                            c.Response.ContentType = "application/json";
                            var result = JsonConvert.SerializeObject(ResponseWrapper.Fail("An unhandled error has occured."));
                            return c.Response.WriteAsync(result);
                        }
                    },
                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        if (!context.Response.HasStarted)
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                            context.Response.ContentType = "application/json";
                            var result = JsonConvert.SerializeObject(ResponseWrapper.Fail("You are not authorized."));
                            return context.Response.WriteAsync(result);
                        }
                        return Task.CompletedTask;
                    },
                    OnForbidden = context =>
                    {

                        context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        context.Response.ContentType = "application/json";
                        var result = JsonConvert.SerializeObject(ResponseWrapper.Fail("You are not authorized to access this resouce."));
                        return context.Response.WriteAsync(result);
                    }
                };
            });
        services.AddAuthorization(options =>
        {
            foreach (var prop in typeof(AppPermissions).GetNestedTypes().SelectMany(c => c.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)))
            {
                var propertyValue = prop.GetValue(null);
                if (propertyValue is not null)
                {
                    options.AddPolicy(propertyValue.ToString(), policy => policy.RequireClaim(AppClaim.Permission, propertyValue.ToString()));
                }
            }
        });

        return services;
    }

    internal static AppConfiguration GetAppConfigurationSettings(this IServiceCollection services, IConfiguration configuration)
    {
        var appConfig = configuration.GetSection(nameof(AppConfiguration));
        services.Configure<AppConfiguration>(appConfig);
        return appConfig.Get<AppConfiguration>();
    }

    internal static IServiceCollection RegisterSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
            {
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = JwtBearerDefaults.AuthenticationScheme,
                BearerFormat = "JWT",
                Description = "Input your Bearer token in this format - Bearer (your token here) to access this API resource."
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = JwtBearerDefaults.AuthenticationScheme,
                        },
                        Scheme = JwtBearerDefaults.AuthenticationScheme,
                        Name = JwtBearerDefaults.AuthenticationScheme,
                        In = ParameterLocation.Header,
                    }, new List<string>()
                },
            });

            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "GREY HR MANAGEMENT SYSTEM",
                License = new OpenApiLicense
                {
                    Name = "MIT License",
                    Url = new Uri("https://opensource/org/licenses/MIT")
                }
            });

        });

        return services;
    }
}
