using ProjectsService.Domain.Entities;
using ProjectsService.Domain.Enums;
using ProjectsService.Domain.Models;

namespace ProjectsService.Domain.Abstractions.Data;

public interface IProjectsRepository
{
    Task<Project?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default,
        bool includeRelatedEntities = false);

    Task<IReadOnlyList<Project>> GetByEmployerUserIdAsync(
        Guid employerUserId,
        CancellationToken cancellationToken = default);
    Task<Project?> GetByEmployerAndTitleAsync(
        Guid employerUserId,
        string title,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProjectModel>> GetByIsActiveAsync(
        bool? isActive,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProjectModel>> GetFilteredAsync(
        Guid? categoryId = null,
        Guid? employerUserId = null,
        Guid? freelancerUserId = null,
        ProjectStatus? projectStatus = null,
        ProjectAcceptanceStatus? acceptanceStatus = null,
        string? searchTitle = null,
        bool? isActive = null,
        int offset = 0,
        int limit = 20,
        CancellationToken cancellationToken = default);

    Task UpdateFreelancerUserIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task UpdateAsync(Project project, CancellationToken cancellationToken = default);
    Task CreateAsync(Project project, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}