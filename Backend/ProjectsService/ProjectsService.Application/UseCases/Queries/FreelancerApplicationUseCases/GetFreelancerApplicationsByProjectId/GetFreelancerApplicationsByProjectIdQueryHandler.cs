using ProjectsService.Application.Constants;
using ProjectsService.Application.Models;
using ProjectsService.Domain.Abstractions.UserContext;

namespace ProjectsService.Application.UseCases.Queries.FreelancerApplicationUseCases.GetFreelancerApplicationsByProjectId;

public class GetFreelancerApplicationsByProjectIdQueryHandler : IRequestHandler<GetFreelancerApplicationsByProjectIdQuery,
    PaginatedResultModel<FreelancerApplication>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    private readonly ILogger<GetFreelancerApplicationsByProjectIdQueryHandler> _logger;

    public GetFreelancerApplicationsByProjectIdQueryHandler(
        IUnitOfWork unitOfWork,
        IUserContext userContext,
        ILogger<GetFreelancerApplicationsByProjectIdQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
        _logger = logger;
    }

    public async Task<PaginatedResultModel<FreelancerApplication>> Handle(
        GetFreelancerApplicationsByProjectIdQuery request,
        CancellationToken cancellationToken)
    {
        var project = await _unitOfWork.ProjectsRepository.GetByIdAsync(request.ProjectId, cancellationToken);

        if (project is null)
        {
            _logger.LogError("Project with ID {ProjectId} not found", request.ProjectId);
            throw new NotFoundException($"Project with ID '{request.ProjectId}' not found");
        }

        var userId = _userContext.GetUserId();
        var isResourceOwner = userId == project.EmployerUserId;
        var isAdmin = _userContext.GetUserRole() == AppRoles.AdminRole;

        if (!isResourceOwner && !isAdmin)
        {
            _logger.LogError("User {UserId} attempted to access project {ProjectId} applications without permission",
                userId, request.ProjectId);
            throw new ForbiddenException($"You do not have access to Project with ID '{request.ProjectId}'");
        }

        var offset = (request.PageNo - 1) * request.PageSize;

        var applications = await _unitOfWork.FreelancerApplicationsRepository.GetAllPaginatedByProjectAsync(
            request.ProjectId,
            offset,
            request.PageSize,
            cancellationToken);

        var applicationsCount = await _unitOfWork.FreelancerApplicationsRepository.CountByProjectAsync(
            request.ProjectId,
            cancellationToken);

        return new PaginatedResultModel<FreelancerApplication>
        {
            Items = applications.ToList(),
            TotalCount = applicationsCount,
            PageNo = request.PageNo,
            PageSize = request.PageSize
        };
    }
}