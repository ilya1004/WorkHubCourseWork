using ProjectsService.Domain.Entities;
using ProjectsService.Domain.Enums;

namespace ProjectsService.Domain.Abstractions.Data;

public interface ILifecyclesRepository
{
    Task<Lifecycle?> GetByProjectIdAsync(
        Guid projectId,
        CancellationToken cancellationToken = default,
        bool includeProject = false);

    Task UpdateStatusByProjectIdAsync(
        Guid projectId,
        ProjectStatus status,
        DateTime updatedAt,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(Lifecycle lifecycle, CancellationToken cancellationToken = default);
    Task CreateAsync(Lifecycle lifecycle, CancellationToken cancellationToken = default);

    Task UpdateProjectStatusAsync(
        Guid projectId,
        ProjectStatus projectStatus,
        DateTime updatedAt,
        CancellationToken cancellationToken = default);

    Task UpdateAcceptanceStatusAsync(
        Guid projectId,
        ProjectAcceptanceStatus projectAcceptanceStatus,
        DateTime updatedAt,
        CancellationToken cancellationToken = default);
}