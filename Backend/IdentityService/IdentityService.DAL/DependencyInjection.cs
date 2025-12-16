using IdentityService.DAL.Abstractions.DbStartupService;
using IdentityService.DAL.Repositories;
using IdentityService.DAL.Services.DbStartupService;
using IdentityService.DAL.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityService.DAL;

public static class DependencyInjection
{
    public static IServiceCollection AddDAL(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("PostgresConnection")));

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("RedisConnection");
        });

        services.AddOptionsWithValidateOnStart<CacheOptions>()
            .BindConfiguration("CacheOptions");

        services.AddScoped<IUnitOfWork, AppUnitOfWork>();
        services.AddScoped<IDbStartupService, DbStartupService>();

        services.AddScoped<ICvLanguagesRepository, CvLanguagesRepository>();
        services.AddScoped<ICvSkillsRepository, CvSkillsRepository>();
        services.AddScoped<ICvsRepository, CvsRepository>();
        services.AddScoped<ICvWorkExperiencesRepository, CvWorkExperiencesRepository>();
        services.AddScoped<IEmployerIndustriesRepository, EmployerIndustriesRepository>();
        services.AddScoped<IEmployerProfilesRepository, EmployerProfilesRepository>();
        services.AddScoped<IFreelancerProfilesRepository, FreelancerProfilesRepository>();
        services.AddScoped<IRolesRepository, RolesRepository>();
        services.AddScoped<IUsersRepository, UsersRepository>();

        services.AddHealthChecks()
            .AddNpgSql(configuration.GetConnectionString("PostgresConnection")!);
            // .AddRedis(configuration.GetConnectionString("RedisConnection")!);

        return services;
    }
}