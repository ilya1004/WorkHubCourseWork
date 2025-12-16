using FluentValidation;
using IdentityService.API.AuthorizationPolicies.AdminOrSelfPolicy;
using IdentityService.BLL.Settings;
using IdentityService.DAL.Constants;
using IdentityService.DAL.Data;
using IdentityService.DAL.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using IdentityService.API.Interceptors;
using IdentityService.API.Middlewares;
using IdentityService.API.Services;
using IdentityService.BLL.Abstractions.UserContext;

namespace IdentityService.API;

public static class DependencyInjection
{
    public static IServiceCollection AddApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers()
            .AddJsonOptions(options => options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

        services.AddTransient<GlobalLoggingMiddleware>();
        services.AddTransient<GlobalExceptionHandlingMiddleware>();
        
        services.AddIdentity<User, IdentityRole<Guid>>()
            .AddDefaultTokenProviders();
        
        services.Configure<DataProtectionTokenProviderOptions>(options =>
        {
            options.TokenLifespan = TimeSpan.FromHours(
                configuration.GetRequiredSection("IdentityTokenExpirationTimeInHours").Get<int>());
        });

        services.AddScoped<IAuthorizationHandler, AdminOrSelfHandler>();

        services.AddOptionsWithValidateOnStart<JwtSettings>()
            .BindConfiguration("JwtSettings");

        var jwtSettings = configuration.GetRequiredSection("JwtSettings").Get<JwtSettings>();

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings!.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddAuthorizationBuilder()
            .AddPolicy(AuthPolicies.AdminPolicy,
                policy => policy.RequireRole(AppRoles.AdminRole))
            .AddPolicy(AuthPolicies.ModeratorPolicy,
                policy => policy.RequireRole(AppRoles.ModeratorRole))
            .AddPolicy(AuthPolicies.FreelancerPolicy,
                policy => policy.RequireRole(AppRoles.FreelancerRole))
            .AddPolicy(AuthPolicies.EmployerPolicy,
                policy => policy.RequireRole(AppRoles.EmployerRole))
            .AddPolicy(AuthPolicies.FreelancerOrEmployerPolicy,
                policy => policy.RequireRole(AppRoles.FreelancerRole, AppRoles.EmployerRole));

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddFluentValidationAutoValidation(config =>
        {
            config.EnableFormBindingSourceAutomaticValidation = true;
            config.EnableBodyBindingSourceAutomaticValidation = true;
            config.EnableQueryBindingSourceAutomaticValidation = true;
        });

        services.AddAutoMapper(config =>
            config.AddMaps(Assembly.GetExecutingAssembly()));

        services.AddScoped<IUserContext, UserContext>();

        services.AddSingleton<ErrorHandlingInterceptor>();
        services.AddSingleton<GrpcLoggingInterceptor>();
            
        services.AddGrpc(options =>
        {
            options.EnableDetailedErrors = true;
            options.Interceptors.Add<GrpcLoggingInterceptor>();
            options.Interceptors.Add<ErrorHandlingInterceptor>();
        });

        return services;
    }
}