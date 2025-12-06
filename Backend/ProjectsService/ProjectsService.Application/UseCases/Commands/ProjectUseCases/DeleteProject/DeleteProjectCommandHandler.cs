using ProjectsService.Application.Constants;
using ProjectsService.Domain.Abstractions.UserContext;

namespace ProjectsService.Application.UseCases.Commands.ProjectUseCases.DeleteProject;

public class DeleteProjectCommandHandler : IRequestHandler<DeleteProjectCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    private readonly ILogger<DeleteProjectCommandHandler> _logger;

    public DeleteProjectCommandHandler(IUnitOfWork unitOfWork,
        IUserContext userContext,
        ILogger<DeleteProjectCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
        _logger = logger;
    }

    public async Task Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await _unitOfWork.ProjectsRepository.GetByIdAsync(
            request.ProjectId, 
            cancellationToken,
            true);
        
        if (project?.Lifecycle is null)
        {
            _logger.LogError("Project {ProjectId} not found", request.ProjectId);
            throw new NotFoundException($"Project with ID '{request.ProjectId}' not found");
        }
        
        var userId = _userContext.GetUserId();
        var isResourceOwned = userId == project.EmployerUserId;
        var isAdmin = _userContext.GetUserRole() == AppRoles.AdminRole;

        if (!isResourceOwned && !isAdmin)
        {
            _logger.LogError("User {UserId} attempted to delete project {ProjectId} without permission",
                userId, request.ProjectId);
            throw new ForbiddenException($"You do not have access to project with ID '{request.ProjectId}'");
        }

        if (project.Lifecycle.ProjectStatus != ProjectStatus.Cancelled)
        {
            _logger.LogWarning("Project {ProjectId} must be cancelled before deletion", request.ProjectId);
            throw new BadRequestException("You need to cancel the project before its removing");
        }

        await _unitOfWork.ProjectsRepository.DeleteAsync(project.Id, cancellationToken);
    }
}
