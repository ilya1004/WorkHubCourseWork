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

    public async Task<EmployerProfile?> GetByUserId(Guid userId, CancellationToken cancellationToken = default)
    {
        try {
            return await _context.EmployerProfiles
                .FromSql($"""
                          SELECT * FROM "EmployerProfiles" WHERE "UserId" = {userId.ToString()}
                          """)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to get user by email. Error: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to get user by email. Error: {ex.Message}");
        }
    }

    public async Task UpdateAsync(
        Guid id,
        EmployerProfile profile,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var rowsAffected = await _context.Database.ExecuteSqlAsync(
                $"""
                 UPDATE "EmployerProfiles"
                 SET "CompanyName" = {profile.CompanyName}, "About" = {profile.About}, "IndustryId" = {profile.IndustryId}  
                 WHERE "Id" = {id.ToString()}
                 """,
                cancellationToken);

            if (rowsAffected != 1)
            {
                _logger.LogError("Failed to update employer profile. Affected [{rowsAffected}] rows", rowsAffected);
                throw new InvalidOperationException($"Failed to update employer profile. Affected [{rowsAffected}] rows");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to update employer profile. Error: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to update employer profile. Error: {ex.Message}");
        }
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