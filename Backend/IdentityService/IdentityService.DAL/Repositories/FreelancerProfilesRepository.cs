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

    public async Task UpdateStripeAccountIdAsync(Guid userId, string stripeAccountId, CancellationToken cancellationToken = default)
    {
        try
        {
            var rowsAffected = await _context.Database.ExecuteSqlAsync(
                $"""
                 UPDATE "FreelancerProfiles"
                 SET "StripeAccountId" = {stripeAccountId}
                 WHERE "UserId" = {userId.ToString()}
                 """,
                cancellationToken);

            if (rowsAffected != 1)
            {
                _logger.LogError("Failed to update freelancer stripe account id. Affected [{rowsAffected}] rows", rowsAffected);
                throw new InvalidOperationException($"Failed to update freelancer stripe account id. Affected [{rowsAffected}] rows");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to update freelancer stripe account id. Error: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to update freelancer stripe account id. Error: {ex.Message}");
        }
    }

    public async Task UpdateAsync(
        Guid id,
        FreelancerProfile profile,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var rowsAffected = await _context.Database.ExecuteSqlAsync(
                $"""
                 UPDATE "FreelancerProfiles"
                 SET "FirstName" = {profile.FirstName}, 
                     "LastName" = {profile.LastName}, 
                     "Nickname" = {profile.Nickname},
                     "About" = {profile.About}
                 WHERE "Id" = {id.ToString()}
                 """,
                cancellationToken);

            if (rowsAffected != 1)
            {
                _logger.LogError("Failed to update freelancer profile. Affected [{rowsAffected}] rows", rowsAffected);
                throw new InvalidOperationException($"Failed to update freelancer profile. Affected [{rowsAffected}] rows");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to update freelancer profile. Error: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to update freelancer profile. Error: {ex.Message}");
        }
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