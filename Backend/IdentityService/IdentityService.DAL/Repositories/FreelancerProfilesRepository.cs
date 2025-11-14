using IdentityService.DAL.Abstractions.Repositories;
using IdentityService.DAL.Data;
using Microsoft.Extensions.Logging;

namespace IdentityService.DAL.Repositories;

public class FreelancerProfilesRepository : IFreelancerProfilesRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<FreelancerProfilesRepository> _logger;

    public FreelancerProfilesRepository(ApplicationDbContext context, ILogger<FreelancerProfilesRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task CreateAsync(FreelancerProfile profile, CancellationToken cancellationToken = default)
    {
        try
        {
            var rowsAffected = await _context.Database.ExecuteSqlAsync(
                $"""
                 INSERT INTO "FreelancerProfiles" ("Id", "FirstName", "LastName", "Nickname", "UserId")
                 VALUES ({profile.Id}, {profile.FirstName}, {profile.LastName}, {profile.Nickname}, {profile.UserId})
                 """,
                cancellationToken);

            if (rowsAffected != 1)
            {
                _logger.LogError("Failed to create freelancer profile. Affected [{rowsAffected}] rows", rowsAffected);
                throw new InvalidOperationException($"Failed to create freelancer profile. Affected [{rowsAffected}] rows");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to create freelancer profile. Error: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to create freelancer profile. Error: {ex.Message}");
        }
    }
}