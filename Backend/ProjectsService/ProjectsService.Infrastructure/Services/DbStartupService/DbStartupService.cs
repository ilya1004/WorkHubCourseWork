using ProjectsService.Domain.Abstractions.StartupServices;
using ProjectsService.Domain.Enums;
using ProjectsService.Infrastructure.Data;

namespace ProjectsService.Infrastructure.Services.DbStartupService;

public class DbStartupService : IDbStartupService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DbStartupService> _logger;
    private readonly ApplicationDbContext _dbContext;

    public DbStartupService(
        IUnitOfWork unitOfWork,
        ILogger<DbStartupService> logger,
        ApplicationDbContext dbContext)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _dbContext = dbContext;
    }

    private readonly Guid _employerUserId = Guid.Parse("019af9e2-46ee-7192-9e22-3a76a357e7b5");
    private readonly Guid _freelancerUserId = Guid.Parse("019af9e3-7dce-72f3-b17a-f8f725edbed3");

    public async Task MakeMigrationsAsync()
    {
        try
        {
            await _dbContext.Database.MigrateAsync();
            _logger.LogInformation("Database migrations applied successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error applying database migrations");
            throw new Exception("Error applying database migrations", ex);
        }
    }

    public async Task InitializeDb()
    {
        var existingCategories = await _unitOfWork.CategoriesRepository.GetAllAsync();

        if (existingCategories.Any())
        {
            _logger.LogInformation("Database already initialized, skipping seeding");
            return;
        }

        var categories = new List<Category>
        {
            new() { Id = Guid.CreateVersion7(), Name = "Web Development" },
            new() { Id = Guid.CreateVersion7(), Name = "Mobile Development" },
            new() { Id = Guid.CreateVersion7(), Name = "Graphic Design" },
            new() { Id = Guid.CreateVersion7(), Name = "Marketing" },
            new() { Id = Guid.CreateVersion7(), Name = "Data Analysis" }
        };

        foreach (var category in categories)
        {
            await _unitOfWork.CategoriesRepository.CreateAsync(category);
        }

        var projects = new List<Project>
        {
            new Project
            {
                Id = Guid.CreateVersion7(),
                Title = "E-commerce Website",
                Description = "Develop a responsive e-commerce website",
                Budget = 5000m,
                CategoryId = categories[0].Id,
                EmployerUserId = _employerUserId,
                IsActive = true,
                Lifecycle = new Lifecycle
                {
                    Id = Guid.CreateVersion7(),
                    CreatedAt = DateTime.UtcNow.AddDays(-2),
                    UpdatedAt = null,
                    ApplicationsStartDate = DateTime.UtcNow,
                    ApplicationsDeadline = DateTime.UtcNow.AddDays(7),
                    WorkStartDate = DateTime.UtcNow.AddDays(10),
                    WorkDeadline = DateTime.UtcNow.AddDays(40),
                    AcceptanceStatus = ProjectAcceptanceStatus.None,
                    ProjectStatus = ProjectStatus.AcceptingApplications,
                },
                FreelancerApplications = new List<FreelancerApplication>
                {
                    new FreelancerApplication
                    {
                        Id = Guid.CreateVersion7(),
                        CreatedAt = DateTime.UtcNow,
                        Status = ApplicationStatus.Pending,
                        FreelancerUserId = _freelancerUserId,
                    }
                },
            },
            new Project
            {
                Id = Guid.CreateVersion7(),
                Title = "Mobile Banking App",
                Description = "Create a secure mobile banking application",
                Budget = 8000m,
                CategoryId = categories[1].Id,
                EmployerUserId = _employerUserId,
                FreelancerUserId = _freelancerUserId,
                IsActive = true,
                Lifecycle = new Lifecycle
                {
                    Id = Guid.CreateVersion7(),
                    CreatedAt = DateTime.UtcNow.AddDays(-10),
                    UpdatedAt = DateTime.UtcNow.AddDays(-2),
                    ApplicationsStartDate = DateTime.UtcNow.AddDays(-9),
                    ApplicationsDeadline = DateTime.UtcNow.AddDays(-2),
                    WorkStartDate = DateTime.UtcNow.AddDays(-2),
                    WorkDeadline = DateTime.UtcNow.AddDays(58),
                    AcceptanceStatus = ProjectAcceptanceStatus.None,
                    ProjectStatus = ProjectStatus.InProgress
                },
                FreelancerApplications = [],
            }
        };

        foreach (var project in projects)
        {
            await _unitOfWork.ProjectsRepository.CreateAsync(project);

            project.Lifecycle!.ProjectId = project.Id;
            await _unitOfWork.LifecyclesRepository.CreateAsync(project.Lifecycle);

            foreach (var application in project.FreelancerApplications)
            {
                application.ProjectId = project.Id;
                await _unitOfWork.FreelancerApplicationsRepository.CreateAsync(application);
            }
        }

        _logger.LogInformation("Database initialization completed successfully");
    }
}