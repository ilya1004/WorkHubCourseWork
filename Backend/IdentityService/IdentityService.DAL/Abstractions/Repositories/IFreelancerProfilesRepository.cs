namespace IdentityService.DAL.Abstractions.Repositories;

public interface IFreelancerProfilesRepository
{
    Task CreateAsync(FreelancerProfile profile, CancellationToken cancellationToken = default);
}