using ProjectsService.Domain.Entities;
using ProjectsService.Domain.Enums;

namespace ProjectsService.Domain.Abstractions.Data;

public interface IFreelancerApplicationsRepository
{
    Task<FreelancerApplication?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<FreelancerApplication>> GetByProjectIdAsync(
        Guid projectId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<FreelancerApplication>> GetByFreelancerUserIdAsync(
        Guid freelancerUserId,
        CancellationToken cancellationToken = default);
    Task<bool> AnyByProjectIdAndApplicationStatus(Guid projectId, ApplicationStatus status, CancellationToken cancellationToken = default);

    Task UpdateRejectedStatusWhenNotAcceptedAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task UpdateAsync(FreelancerApplication application, CancellationToken cancellationToken = default);
    Task CreateAsync(FreelancerApplication application, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}