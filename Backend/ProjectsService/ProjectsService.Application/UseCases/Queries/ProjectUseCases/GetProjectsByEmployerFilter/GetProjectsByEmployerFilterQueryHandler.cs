using ProjectsService.Application.Models;
using ProjectsService.Application.Specifications.ProjectSpecifications;
using ProjectsService.Domain.Abstractions.UserContext;
using ProjectsService.Domain.Models;

namespace ProjectsService.Application.UseCases.Queries.ProjectUseCases.GetProjectsByEmployerFilter;

public class GetProjectsByEmployerFilterQueryHandler : IRequestHandler<GetProjectsByEmployerFilterQuery, PaginatedResultModel<ProjectInfo>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;

    public GetProjectsByEmployerFilterQueryHandler(
        IUnitOfWork unitOfWork,
        IUserContext userContext)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
    }

    public async Task<PaginatedResultModel<ProjectInfo>> Handle(GetProjectsByEmployerFilterQuery request, CancellationToken cancellationToken)
    {
        var userId = _userContext.GetUserId();

        var offset = (request.PageNo - 1) * request.PageSize;

        var projects = await _unitOfWork.ProjectsRepository.GetFilteredAsync(
            categoryId: null,
            employerUserId: userId,
            freelancerUserId: null,
            projectStatus: null,
            acceptanceStatus: null,
            searchTitle:  null,
            isActive: null,
            updatedAtStartDate: request.UpdatedAtStartDate,
            updatedAtEndDate: request.UpdatedAtEndDate,
            budgetFrom: null,
            budgetTo: null,
            offset: offset,
            limit: request.PageSize,
            cancellationToken);

        var projectsCount = await _unitOfWork.ProjectsRepository.CountByFilteredAsync(
            categoryId: null,
            employerUserId: userId,
            freelancerUserId: null,
            projectStatus: null,
            acceptanceStatus: null,
            searchTitle:  null,
            isActive: null,
            updatedAtStartDate: request.UpdatedAtStartDate,
            updatedAtEndDate: request.UpdatedAtEndDate,
            budgetFrom: null,
            budgetTo: null,
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