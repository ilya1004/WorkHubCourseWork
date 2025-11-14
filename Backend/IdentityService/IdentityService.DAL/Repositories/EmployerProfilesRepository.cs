using IdentityService.DAL.Abstractions.Repositories;
using IdentityService.DAL.Data;
using Microsoft.Extensions.Logging;

namespace IdentityService.DAL.Repositories;

public class EmployerProfilesRepository : IEmployerProfilesRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<EmployerProfilesRepository> _logger;

    public EmployerProfilesRepository(ApplicationDbContext context, ILogger<EmployerProfilesRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task CreateAsync(EmployerProfile profile, CancellationToken cancellationToken = default)
    {
        try
        {
            var rowsAffected = await _context.Database.ExecuteSqlAsync(
                $"""
                 INSERT INTO "EmployerProfiles" ("Id", "CompanyName", "UserId")
                 VALUES ({profile.Id}, {profile.CompanyName}, {profile.UserId})
                 """,
                cancellationToken);

            if (rowsAffected != 1)
            {
                _logger.LogError("Failed to create employer profile. Affected [{rowsAffected}] rows", rowsAffected);
                throw new InvalidOperationException($"Failed to create employer profile. Affected [{rowsAffected}] rows");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to create employer profile. Error: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to create employer profile. Error: {ex.Message}");
        }
    }
}