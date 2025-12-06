using ProjectsService.Domain.Abstractions.UserContext;

namespace ProjectsService.Application.UseCases.Commands.FreelancerApplicationUseCases.RejectFreelancerApplication;

public class RejectFreelancerApplicationCommandHandler : IRequestHandler<RejectFreelancerApplicationCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    private readonly ILogger<RejectFreelancerApplicationCommandHandler> _logger;

    public RejectFreelancerApplicationCommandHandler(
        IUnitOfWork unitOfWork,
        IUserContext userContext,
        ILogger<RejectFreelancerApplicationCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
        _logger = logger;
    }

    public async Task Handle(RejectFreelancerApplicationCommand request, CancellationToken cancellationToken)
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
            _logger.LogError("User {UserId} attempted to access project {ProjectId} without permission", userId,
                request.ProjectId);
            throw new ForbiddenException($"You do not have access to project with ID '{request.ProjectId}'");
        }

        if (project.Lifecycle.ProjectStatus != ProjectStatus.AcceptingApplications)
        {
            _logger.LogError("Invalid project status {Status} for rejecting applications",
                project.Lifecycle.ProjectStatus);
            throw new BadRequestException(
                "You can reject applications to this project only during accepting applications stage");
        }

        var freelancerApplication = await _unitOfWork.FreelancerApplicationsRepository.GetByIdAsync(
            request.ApplicationId,
            cancellationToken);

        if (freelancerApplication is null)
        {
            _logger.LogError("Freelancer application {ApplicationId} not found", request.ApplicationId);
            throw new NotFoundException($"Freelancer application with ID '{request.ApplicationId}' not found");
        }

        if (freelancerApplication.Status != ApplicationStatus.Accepted)
        {
            _logger.LogError("Freelancer application {ApplicationId} has invalid status {Status}",
                request.ApplicationId, freelancerApplication.Status);
            throw new BadRequestException("Freelancer application status is not accepted");
        }

        freelancerApplication.Status = ApplicationStatus.Pending;

        await _unitOfWork.FreelancerApplicationsRepository.UpdateAsync(freelancerApplication, cancellationToken);
    }
}