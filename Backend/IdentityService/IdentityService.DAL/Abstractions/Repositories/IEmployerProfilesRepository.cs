namespace IdentityService.DAL.Abstractions.Repositories;

public interface IEmployerProfilesRepository
{
    Task CreateAsync(EmployerProfile profile, CancellationToken cancellationToken = default);
}