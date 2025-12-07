using IdentityService.DAL.Abstractions.DbStartupService;
using IdentityService.DAL.Abstractions.PasswordHasher;
using IdentityService.DAL.Constants;

namespace IdentityService.DAL.Services.DbStartupService;

public class DbStartupService : IDbStartupService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DbStartupService> _logger;
    private readonly IPasswordHasher _passwordHasher;

    private readonly Guid _employerUserId = Guid.Parse("019af9e2-46ee-7192-9e22-3a76a357e7b5");
    private readonly Guid _freelancerUserId = Guid.Parse("019af9e3-7dce-72f3-b17a-f8f725edbed3");

    public DbStartupService(
        IUnitOfWork unitOfWork,
        ILogger<DbStartupService> logger,
        IPasswordHasher passwordHasher)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _passwordHasher = passwordHasher;
    }

    public async Task InitializeDb()
    {
        _logger.LogInformation("Starting database initialization...");

        var existingAdminRole = await _unitOfWork.RolesRepository.GetByNameAsync(AppRoles.AdminRole);

        if (existingAdminRole is not null)
        {
            return;
        }

        var adminRole = new Role
        {
            Id = Guid.CreateVersion7(),
            Name = AppRoles.AdminRole
        };

        var moderatorRole = new Role
        {
            Id = Guid.CreateVersion7(),
            Name = AppRoles.ModeratorRole
        };

        var employerRole = new Role
        {
            Id = Guid.CreateVersion7(),
            Name = AppRoles.EmployerRole
        };

        var freelancerRole = new Role
        {
            Id = Guid.CreateVersion7(),
            Name = AppRoles.FreelancerRole
        };

        await _unitOfWork.RolesRepository.CreateAsync(adminRole);
        await _unitOfWork.RolesRepository.CreateAsync(moderatorRole);
        await _unitOfWork.RolesRepository.CreateAsync(employerRole);
        await _unitOfWork.RolesRepository.CreateAsync(freelancerRole);


        var admin = new User
        {
            Id = Guid.CreateVersion7(),
            RegisteredAt = DateTime.UtcNow,
            Email = "admin@gmail.com",
            PasswordHash = _passwordHasher.HashPassword("Admin_123"),
            IsEmailConfirmed = true,
            RoleId = adminRole.Id
        };

        await _unitOfWork.UsersRepository.CreateAsync(admin);


        var moderator = new User
        {
            Id = Guid.CreateVersion7(),
            RegisteredAt = DateTime.UtcNow,
            Email = "moderator@gmail.com",
            PasswordHash = _passwordHasher.HashPassword("Moderator_123"),
            IsEmailConfirmed = true,
            RoleId = moderatorRole.Id
        };

        await _unitOfWork.UsersRepository.CreateAsync(moderator);


        var freelancer = new User
        {
            Id = _freelancerUserId,
            RegisteredAt = DateTime.UtcNow,
            Email = "ilya@gmail.com",
            PasswordHash = _passwordHasher.HashPassword("Ilya_123"),
            IsEmailConfirmed = true,
            RoleId = freelancerRole.Id
        };

        await _unitOfWork.UsersRepository.CreateAsync(freelancer);

        var freelancerProfile = new FreelancerProfile
        {
            Id = Guid.CreateVersion7(),
            FirstName = "Ilya",
            LastName = "Rabets",
            Nickname = "Moonlight",
            UserId = freelancer.Id
        };

        await _unitOfWork.FreelancerProfilesRepository.CreateAsync(freelancerProfile);


        var employer = new User
        {
            Id = _employerUserId,
            RegisteredAt = DateTime.UtcNow,
            Email = "pavlusha@gmail.com",
            PasswordHash = _passwordHasher.HashPassword("Pavlusha_123"),
            IsEmailConfirmed = true,
            RoleId = employerRole.Id
        };

        await _unitOfWork.UsersRepository.CreateAsync(employer);

        var employerProfile = new EmployerProfile
        {
            Id = Guid.CreateVersion7(),
            CompanyName = "Sunrise Company",
            UserId = employer.Id
        };

        await _unitOfWork.EmployerProfilesRepository.CreateAsync(employerProfile);


        var employerIndustries = new List<EmployerIndustry>
        {
            new() { Id = Guid.CreateVersion7(), Name = "IT & Software" },
            new() { Id = Guid.CreateVersion7(), Name = "Marketing & Advertising" },
            new() { Id = Guid.CreateVersion7(), Name = "Finance & Accounting" },
            new() { Id = Guid.CreateVersion7(), Name = "Healthcare & Medicine" },
            new() { Id = Guid.CreateVersion7(), Name = "Education & Training" },
            new() { Id = Guid.CreateVersion7(), Name = "Construction & Engineering" },
            new() { Id = Guid.CreateVersion7(), Name = "E-commerce & Retail" }
        };

        foreach (var item in employerIndustries)
        {
            await _unitOfWork.EmployerIndustriesRepository.CreateAsync(item);
        }

        _logger.LogInformation("Database initialization completed successfully");
    }
}