namespace IdentityService.DAL.Abstractions.Repositories;

public interface ICvsRepository
{
    Task<Cv?> GetByIdAsync(Guid cvId, CancellationToken cancellationToken = default, bool includeRelated = false);
    Task<IReadOnlyList<Cv>> GetByFreelancerIdAsync(Guid freelancerUserId, CancellationToken cancellationToken = default);
    Task<Cv?> GetPublicByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Cv>> GetAllPaginatedAsync(
        int offset,
        int limit,
        CancellationToken cancellationToken = default);
    Task CreateAsync(Cv cv, CancellationToken cancellationToken = default);
    Task UpdateAsync(Cv cv, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}