namespace IdentityService.DAL.Abstractions.Repositories;

public interface IFreelancerProfilesRepository
{
    Task UpdateAsync(Guid id, FreelancerProfile profile, CancellationToken cancellationToken = default);
    Task CreateAsync(FreelancerProfile profile, CancellationToken cancellationToken = default);
}