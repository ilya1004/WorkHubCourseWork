using IdentityService.DAL.Abstractions.Repositories;
using IdentityService.DAL.Data;

namespace IdentityService.DAL.Repositories;

public class RolesRepository(ApplicationDbContext context) : IRolesRepository
{
    public async Task<Role?> GetByNameAsync(string roleName, CancellationToken cancellationToken = default)
    {
        return await context.Roles
            .FromSql($"""
                      SELECT * FROM "Roles" WHERE "Name" = {roleName}
                      """)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);
    }
}