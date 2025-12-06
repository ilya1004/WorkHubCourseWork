using ProjectsService.Domain.Abstractions.UserContext;

namespace ProjectsService.Application.UseCases.Commands.ProjectUseCases.UpdateAcceptanceStatus;

public class UpdateAcceptanceStatusCommandHandler : IRequestHandler<UpdateAcceptanceStatusCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    private readonly ILogger<UpdateAcceptanceStatusCommandHandler> _logger;

    public UpdateAcceptanceStatusCommandHandler(IUnitOfWork unitOfWork,
        IUserContext userContext,
        ILogger<UpdateAcceptanceStatusCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
        _logger = logger;
    }

    public async Task Handle(UpdateAcceptanceStatusCommand request, CancellationToken cancellationToken)
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

        if (project.EmployerUserId != userId)
        {
            _logger.LogError("User {UserId} attempted to update acceptance status for project {ProjectId} without permission",
                userId, request.ProjectId);
            throw new ForbiddenException($"You do not have access to project with ID '{request.ProjectId}'");
        }

        if (project.Lifecycle.AcceptanceStatus != ProjectAcceptanceStatus.Requested)
        {
            _logger.LogError("Acceptance not requested for project {ProjectId}", request.ProjectId);
            throw new BadRequestException("Project acceptance is not requested yet");
        }

        if (project.Lifecycle.ProjectStatus != ProjectStatus.PendingForReview)
        {
            _logger.LogError("Invalid project status {Status} for acceptance update", project.Lifecycle.ProjectStatus);
            throw new BadRequestException("Current project status do not allow you to update acceptance status");
        }

        var lifecycle = project.Lifecycle;
        
        if (request.IsAcceptanceConfirmed)
        {
            lifecycle.AcceptanceStatus = ProjectAcceptanceStatus.Accepted;
            lifecycle.ProjectStatus = ProjectStatus.Completed;
        }
        else
        {
            lifecycle.AcceptanceStatus = ProjectAcceptanceStatus.None;

            if (DateTime.UtcNow > project.Lifecycle.WorkDeadline)
            {
                lifecycle.ProjectStatus = ProjectStatus.Expired;
            }
            else
            {
                lifecycle.ProjectStatus = ProjectStatus.InProgress;
            }
        }
        
        await _unitOfWork.LifecyclesRepository.UpdateAsync(lifecycle, cancellationToken);
    }
}