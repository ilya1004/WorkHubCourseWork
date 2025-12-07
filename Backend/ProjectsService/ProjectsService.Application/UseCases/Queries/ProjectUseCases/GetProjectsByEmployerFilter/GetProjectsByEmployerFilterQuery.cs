using ProjectsService.Application.Models;
using ProjectsService.Domain.Models;

namespace ProjectsService.Application.UseCases.Queries.ProjectUseCases.GetProjectsByEmployerFilter;

public sealed record GetProjectsByEmployerFilterQuery(
    DateTime? UpdatedAtStartDate,
    DateTime? UpdatedAtEndDate,
    ProjectStatus? ProjectStatus,
    ProjectAcceptanceStatus? ProjectAcceptanceStatus,
    int PageNo = 1,
    int PageSize = 10) : IRequest<PaginatedResultModel<ProjectInfo>>;