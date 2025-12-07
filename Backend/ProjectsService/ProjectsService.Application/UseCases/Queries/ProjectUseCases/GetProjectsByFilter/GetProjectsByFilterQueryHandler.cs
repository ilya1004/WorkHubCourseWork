using ProjectsService.Application.Models;
using ProjectsService.Application.Specifications.ProjectSpecifications;
using ProjectsService.Domain.Models;

namespace ProjectsService.Application.UseCases.Queries.ProjectUseCases.GetProjectsByFilter;

public class GetProjectsByFilterQueryHandler : IRequestHandler<GetProjectsByFilterQuery, PaginatedResultModel<ProjectInfo>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetProjectsByFilterQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PaginatedResultModel<ProjectInfo>> Handle(GetProjectsByFilterQuery request, CancellationToken cancellationToken)
    {
        var offset = (request.PageNo - 1) * request.PageSize;

        var projects = await _unitOfWork.ProjectsRepository.GetFilteredAsync(
            categoryId: request.CategoryId,
            employerUserId: request.EmployerId,
            freelancerUserId: null,
            projectStatus: request.ProjectStatus,
            acceptanceStatus: null,
            searchTitle: request.Title,
            isActive: null,
            updatedAtStartDate: null,
            updatedAtEndDate: null,
            budgetFrom: request.BudgetFrom,
            budgetTo: request.BudgetTo,
            offset: offset,
            limit: request.PageSize,
            cancellationToken);

        var projectsCount = await _unitOfWork.ProjectsRepository.CountByFilteredAsync(
            categoryId: request.CategoryId,
            employerUserId: request.EmployerId,
            freelancerUserId: null,
            projectStatus: request.ProjectStatus,
            acceptanceStatus: null,
            searchTitle: request.Title,
            isActive: null,
            updatedAtStartDate: null,
            updatedAtEndDate: null,
            budgetFrom: request.BudgetFrom,
            budgetTo: request.BudgetTo,
            cancellationToken);

        return new PaginatedResultModel<ProjectInfo>
        {
            Items = projects.ToList(),
            TotalCount = projectsCount,
            PageNo = request.PageNo,
            PageSize = request.PageSize
        };
    }
}