namespace IdentityService.DAL.Abstractions.Repositories;

public interface IFreelancerProfilesRepository
{
    Task UpdateStripeAccountIdAsync(Guid userId, string stripeAccountId, CancellationToken cancellationToken = default);
    Task UpdateAsync(Guid id, FreelancerProfile profile, CancellationToken cancellationToken = default);
    Task CreateAsync(FreelancerProfile profile, CancellationToken cancellationToken = default);
}