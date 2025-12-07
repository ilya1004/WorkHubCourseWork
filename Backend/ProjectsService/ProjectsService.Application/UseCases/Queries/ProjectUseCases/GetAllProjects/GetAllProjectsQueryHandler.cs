using ProjectsService.Application.Models;
using ProjectsService.Domain.Models;

namespace ProjectsService.Application.UseCases.Queries.ProjectUseCases.GetAllProjects;

public class GetAllProjectsQueryHandler : IRequestHandler<GetAllProjectsQuery, PaginatedResultModel<ProjectInfo>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllProjectsQueryHandler(
        IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PaginatedResultModel<ProjectInfo>> Handle(GetAllProjectsQuery request, CancellationToken cancellationToken)
    {
        var offset = (request.PageNo - 1) * request.PageSize;

        var projects = await _unitOfWork.ProjectsRepository.PaginatedListAllAsync(
            offset, request.PageSize, cancellationToken);

        var projectsCount = await _unitOfWork.ProjectsRepository.CountAllAsync(cancellationToken);

        return new PaginatedResultModel<ProjectInfo>
        {
            Items = projects.ToList(),
            TotalCount = projectsCount,
            PageSize = request.PageSize,
            PageNo = request.PageNo
        };
    }
}