namespace IdentityService.DAL.Abstractions.Repositories;

public interface IEmployerProfilesRepository
{
    Task<EmployerProfile?> GetByUserId(Guid userId, CancellationToken cancellationToken = default);
    Task UpdateAsync(Guid id, EmployerProfile profile, CancellationToken cancellationToken = default);
    Task CreateAsync(EmployerProfile profile, CancellationToken cancellationToken = default);
}