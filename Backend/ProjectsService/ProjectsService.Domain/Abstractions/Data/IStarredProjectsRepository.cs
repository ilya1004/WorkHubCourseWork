using ProjectsService.Domain.Entities;

namespace ProjectsService.Domain.Abstractions.Data;

public interface IStarredProjectsRepository
{
    Task<bool> IsStarredAsync(
        Guid projectId,
        Guid freelancerUserId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<StarredProject>> GetByFreelancerUserIdAsync(
        Guid freelancerUserId,
        CancellationToken cancellationToken = default);

    Task CreateAsync(StarredProject starredProject, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}