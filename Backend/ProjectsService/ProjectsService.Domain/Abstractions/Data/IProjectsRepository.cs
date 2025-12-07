using ProjectsService.Domain.Entities;
using ProjectsService.Domain.Enums;
using ProjectsService.Domain.Models;

namespace ProjectsService.Domain.Abstractions.Data;

public interface IProjectsRepository
{
    Task<Project?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default,
        bool includeRelatedEntities = false,
        bool includeRelatedCollections = false);

    Task<IReadOnlyList<ProjectInfo>> PaginatedListAllAsync(int offset, int limit, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Project>> GetByEmployerUserIdAsync(
        Guid employerUserId,
        CancellationToken cancellationToken = default);

    Task<Project?> GetByEmployerAndTitleAsync(
        Guid employerUserId,
        string title,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProjectInfo>> GetByIsActiveAsync(
        bool? isActive,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProjectInfo>> GetFilteredAsync(
        Guid? categoryId = null,
        Guid? employerUserId = null,
        Guid? freelancerUserId = null,
        ProjectStatus? projectStatus = null,
        ProjectAcceptanceStatus? acceptanceStatus = null,
        string? searchTitle = null,
        bool? isActive = null,
        DateTime? updatedAtStartDate = null,
        DateTime? updatedAtEndDate = null,
        decimal? budgetFrom = null,
        decimal? budgetTo = null,
        int offset = 0,
        int limit = 10,
        CancellationToken cancellationToken = default);

    Task<int> CountByFilteredAsync(
        Guid? categoryId = null,
        Guid? employerUserId = null,
        Guid? freelancerUserId = null,
        ProjectStatus? projectStatus = null,
        ProjectAcceptanceStatus? acceptanceStatus = null,
        string? searchTitle = null,
        bool? isActive = null,
        DateTime? updatedAtStartDate = null,
        DateTime? updatedAtEndDate = null,
        decimal? budgetFrom = null,
        decimal? budgetTo = null,
        CancellationToken cancellationToken = default);


    Task<int> CountAllAsync(CancellationToken cancellationToken = default);
    Task UpdateFreelancerUserIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task UpdateAsync(Project project, CancellationToken cancellationToken = default);
    Task UpdatePaymentIntentAsync(Guid id, string paymentIntentId, CancellationToken cancellationToken = default);
    Task CreateAsync(Project project, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}