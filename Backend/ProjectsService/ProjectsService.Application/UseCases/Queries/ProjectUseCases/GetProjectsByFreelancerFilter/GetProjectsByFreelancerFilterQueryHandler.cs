using ProjectsService.Application.Models;
using ProjectsService.Domain.Abstractions.UserContext;
using ProjectsService.Domain.Models;

namespace ProjectsService.Application.UseCases.Queries.ProjectUseCases.GetProjectsByFreelancerFilter;

public class GetProjectsByFreelancerFilterQueryHandler : IRequestHandler<GetProjectsByFreelancerFilterQuery,
    PaginatedResultModel<ProjectInfo>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;

    public GetProjectsByFreelancerFilterQueryHandler(
        IUnitOfWork unitOfWork,
        IUserContext userContext)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
    }

    public async Task<PaginatedResultModel<ProjectInfo>> Handle(
        GetProjectsByFreelancerFilterQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _userContext.GetUserId();

        var offset = (request.PageNo - 1) * request.PageSize;

        var projects = await _unitOfWork.ProjectsRepository.GetFilteredAsync(
            categoryId: null,
            employerUserId: request.EmployerId,
            freelancerUserId: userId,
            projectStatus: request.ProjectStatus,
            acceptanceStatus: null,
            searchTitle: null,
            isActive: null,
            updatedAtStartDate: null,
            updatedAtEndDate: null,
            budgetFrom: null,
            budgetTo: null,
            offset: offset,
            limit: request.PageSize,
            cancellationToken);

        var projectsCount = await _unitOfWork.ProjectsRepository.CountByFilteredAsync(
            categoryId: null,
            employerUserId: request.EmployerId,
            freelancerUserId: userId,
            projectStatus: request.ProjectStatus,
            acceptanceStatus: null,
            searchTitle: null,
            isActive: null,
            updatedAtStartDate: null,
            updatedAtEndDate: null,
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