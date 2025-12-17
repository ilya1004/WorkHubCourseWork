using System.Reflection;
using Confluent.Kafka;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectsService.Domain.Abstractions.KafkaProducerServices;
using ProjectsService.Domain.Abstractions.StartupServices;
using ProjectsService.Infrastructure.Data;
using ProjectsService.Infrastructure.Repositories;
using ProjectsService.Infrastructure.Services.DbStartupService;
using ProjectsService.Infrastructure.Services.HangfireJobsInitializer;
using ProjectsService.Infrastructure.Services.KafkaConsumerServices;
using ProjectsService.Infrastructure.Services.KafkaProducerServices;
using Serilog;

namespace ProjectsService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("PostgresConnectionDb"))
                .LogTo(Console.WriteLine)
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors());

        services.AddScoped<IUnitOfWork, AppUnitOfWork>();

        services.AddHangfire(config =>
            config.UsePostgreSqlStorage(options =>
                options.UseNpgsqlConnection(configuration.GetConnectionString("PostgresConnectionHangfireDb"))));

        services.AddHangfireServer();

        services.AddAutoMapper(config =>
            config.AddMaps(Assembly.GetExecutingAssembly()));

        services.AddOptionsWithValidateOnStart<CacheOptions>()
            .BindConfiguration("CacheOptions");

        services.AddScoped<IRecurringJobManager, RecurringJobManager>();
        services.AddScoped<IDbStartupService, DbStartupService>();
        services.AddScoped<IBackgroundJobsInitializer, HangfireJobsInitializer>();

        services.AddScoped<ICategoriesRepository, CategoriesRepository>();
        services.AddScoped<IFreelancerApplicationsRepository, FreelancerApplicationsRepository>();
        services.AddScoped<ILifecyclesRepository, LifecyclesRepository>();
        services.AddScoped<IProjectsRepository, ProjectsRepository>();
        services.AddScoped<IReportsRepository, ReportsRepository>();
        services.AddScoped<IStarredProjectsRepository, StarredProjectsRepository>();

        services.AddOptionsWithValidateOnStart<KafkaSettings>()
            .BindConfiguration("KafkaSettings");

        services.AddSingleton<IPaymentsProducerService, PaymentsProducerService>();

        services.AddHostedService<PaymentsConsumerService>();

        services.AddHealthChecks()
            .AddNpgSql(configuration.GetConnectionString("PostgresConnectionDb")!, name: "postgres-projects")
            .AddRedis(configuration.GetConnectionString("RedisConnection")!)
            .AddKafka(new ProducerConfig
            {
                BootstrapServers = configuration["KafkaSettings:BootstrapServers"]
            }, name: "kafka");

        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();

        services.AddLogging(logging => logging.AddSerilog());

        return services;
    }
}