using ProjectsService.Domain.Entities;
using ProjectsService.Domain.Enums;

namespace ProjectsService.Domain.Abstractions.Data;

public interface IFreelancerApplicationsRepository
{
    Task<FreelancerApplication?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default, bool includeRelatedEntities = false);

    Task<IReadOnlyList<FreelancerApplication>> GetByProjectIdAsync(
        Guid projectId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<FreelancerApplication>> GetByFreelancerUserIdAsync(
        Guid freelancerUserId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<FreelancerApplication>> GetAllPaginatedAsync(int offset, int limit, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<FreelancerApplication>> GetAllPaginatedByProjectAsync(
        Guid projectId,
        int offset,
        int limit,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<FreelancerApplication>> GetByFilterAsync(
        DateTime? startDate,
        DateTime? endDate,
        ApplicationStatus? applicationStatus,
        int limit,
        int offset,
        CancellationToken cancellationToken = default);

    Task<int> CountByFilterAsync(
        DateTime? startDate,
        DateTime? endDate,
        ApplicationStatus? applicationStatus,
        int limit,
        int offset,
        CancellationToken cancellationToken = default);

    Task<int> CountAllAsync(CancellationToken cancellationToken = default);
    Task<int> CountByProjectAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<bool> AnyByProjectIdAndApplicationStatus(Guid projectId, ApplicationStatus status, CancellationToken cancellationToken = default);

    Task UpdateRejectedStatusWhenNotAcceptedAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task UpdateAsync(FreelancerApplication application, CancellationToken cancellationToken = default);
    Task CreateAsync(FreelancerApplication application, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}