using IdentityService.DAL.Abstractions.Repositories;
using IdentityService.DAL.Data;

namespace IdentityService.DAL.Repositories;

public class FreelancerProfilesRepository(ApplicationDbContext context) : IFreelancerProfilesRepository
{
    public async Task<bool> CreateAsync(FreelancerProfile profile, CancellationToken cancellationToken = default)
    {
        var rowsAffected = await context.Database.ExecuteSqlAsync(
            $"""
             INSERT INTO "FreelancerProfiles" (Id, RegisteredAt, Email, PasswordHash, RoleId)
             VALUES ({user.Id}, {user.RegisteredAt}, {user.Email}, {user.PasswordHash}, {user.RoleId}
             """,
            cancellationToken);

        return rowsAffected == 1;
    }
}