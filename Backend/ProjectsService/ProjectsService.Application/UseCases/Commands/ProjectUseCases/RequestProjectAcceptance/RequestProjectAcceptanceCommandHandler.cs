using ProjectsService.Application.UseCases.Commands.ProjectUseCases.UpdateAcceptanceRequest;
using ProjectsService.Domain.Abstractions.UserContext;

namespace ProjectsService.Application.UseCases.Commands.ProjectUseCases.RequestProjectAcceptance;

public class RequestProjectAcceptanceCommandHandler : IRequestHandler<RequestProjectAcceptanceCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    private readonly ILogger<RequestProjectAcceptanceCommandHandler> _logger;

    public RequestProjectAcceptanceCommandHandler(IUnitOfWork unitOfWork,
        IUserContext userContext,
        ILogger<RequestProjectAcceptanceCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
        _logger = logger;
    }

    public async Task Handle(RequestProjectAcceptanceCommand request, CancellationToken cancellationToken)
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
        
        if (project.FreelancerUserId != userId)
        {
            _logger.LogError("User {UserId} attempted to update acceptance for project {ProjectId} without permission",
                userId, request.ProjectId);
            throw new ForbiddenException($"You do not have access to project with ID '{request.ProjectId}'");
        }
        
        if (project.Lifecycle.ProjectStatus != ProjectStatus.InProgress && 
            project.Lifecycle.ProjectStatus != ProjectStatus.Expired)
        {
            _logger.LogError("Invalid project status {Status} for acceptance request", project.Lifecycle.ProjectStatus);
            throw new BadRequestException("Current project status do not allow you to send acceptance request");
        }

        var lifecycle = project.Lifecycle;

        lifecycle.AcceptanceStatus = ProjectAcceptanceStatus.Requested;
        lifecycle.ProjectStatus = ProjectStatus.PendingForReview;
        lifecycle.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.LifecyclesRepository.UpdateAsync(lifecycle, cancellationToken);
    }
}