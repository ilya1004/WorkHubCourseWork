using ProjectsService.Application.Models;
using ProjectsService.Domain.Models;

namespace ProjectsService.Application.UseCases.Queries.ProjectUseCases.GetAllProjects;

public record GetAllProjectsQuery(int PageNo, int PageSize) : IRequest<PaginatedResultModel<ProjectInfo>>;