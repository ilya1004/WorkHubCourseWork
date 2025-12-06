using ProjectsService.Domain.Abstractions.UserContext;

namespace ProjectsService.Application.UseCases.Commands.FreelancerApplicationUseCases.AcceptFreelancerApplication;

public class AcceptFreelancerApplicationCommandHandler : IRequestHandler<AcceptFreelancerApplicationCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    private readonly ILogger<AcceptFreelancerApplicationCommandHandler> _logger;

    public AcceptFreelancerApplicationCommandHandler(
        IUnitOfWork unitOfWork,
        IUserContext userContext,
        ILogger<AcceptFreelancerApplicationCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
        _logger = logger;
    }

    public async Task Handle(AcceptFreelancerApplicationCommand request, CancellationToken cancellationToken)
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

        if (project.FreelancerUserId is not null)
        {
            _logger.LogError("Project {ProjectId} already has freelancer assigned", request.ProjectId);
            throw new BadRequestException("This project already has freelancer to work on it");
        }

        if (project.Lifecycle.ProjectStatus != ProjectStatus.AcceptingApplications)
        {
            _logger.LogError("Invalid project status {Status} for accepting applications",
                project.Lifecycle.ProjectStatus);
            throw new BadRequestException(
                "You can accept applications to this project only during accepting applications stage");
        }

        var hasAcceptedApplication =
            await _unitOfWork.FreelancerApplicationsRepository.AnyByProjectIdAndApplicationStatus(
                project.Id, ApplicationStatus.Accepted, cancellationToken);

        if (hasAcceptedApplication)
        {
            _logger.LogError("You already has accepted freelancer application to this project with ID '{ProjectId}'",
                project.Id);
            throw new BadRequestException(
                $"You already has accepted freelancer application to this project with ID '{project.Id}'");
        }

        var freelancerApplication = await _unitOfWork.FreelancerApplicationsRepository.GetByIdAsync(
            request.ApplicationId, cancellationToken);

        if (freelancerApplication is null)
        {
            _logger.LogError("Freelancer application {ApplicationId} not found", request.ApplicationId);
            throw new NotFoundException($"Freelancer application with ID '{request.ApplicationId}' not found");
        }

        if (freelancerApplication.Status != ApplicationStatus.Pending)
        {
            _logger.LogError("Freelancer application {ApplicationId} has invalid status {Status}",
                request.ApplicationId, freelancerApplication.Status);
            throw new BadRequestException("Freelancer application status is not pending");
        }

        freelancerApplication.Status = ApplicationStatus.Accepted;

        await _unitOfWork.FreelancerApplicationsRepository.UpdateAsync(freelancerApplication, cancellationToken);
    }
}