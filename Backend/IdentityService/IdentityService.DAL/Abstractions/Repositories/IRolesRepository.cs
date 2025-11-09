namespace IdentityService.DAL.Abstractions.Repositories;

public interface IRolesRepository
{
    Task<Role?> GetByNameAsync(string roleName, CancellationToken cancellationToken = default);
}