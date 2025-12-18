using Microsoft.Extensions.Options;
using ProjectsService.Application.Settings;

namespace ProjectsService.Application.BackgroundJobs.UpdateProjectStatuses;

public class UpdateProjectStatusesCommandHandler : IRequestHandler<UpdateProjectStatusesCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOptions<ProjectsSettings> _options;
    private readonly ILogger<UpdateProjectStatusesCommandHandler> _logger;

    public UpdateProjectStatusesCommandHandler(
        IUnitOfWork unitOfWork,
        IOptions<ProjectsSettings> options,
        ILogger<UpdateProjectStatusesCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _options = options;
        _logger = logger;
    }

    public async Task Handle(UpdateProjectStatusesCommand command, CancellationToken cancellationToken)
    {
        var projectModels = await _unitOfWork.ProjectsRepository.GetByIsActiveAsync(
            false,
            cancellationToken);

        foreach (var project in projectModels)
        {
            var previousStatus = project.ProjectStatus;
            var newStatus = project.ProjectStatus;
            var now = DateTime.UtcNow;

            _logger.LogInformation("Processing project {ProjectId}, current status: {CurrentStatus}",
                project.Id, previousStatus);

            // if (lifecycle.AcceptanceStatus == ProjectAcceptanceStatus.Accepted)
            // {
            //     lifecycle.ProjectStatus = ProjectStatus.Completed;
            //
            //     logger.LogInformation("Project {ProjectId} marked as Completed (AcceptanceConfirmed)", project.Id);
            // }

            var isProjectHasAcceptedFreelancerApplications =
                await IsProjectHasAcceptedFreelancerApplications(project.Id, cancellationToken);

            if (now > project.WorkDeadline &&
                project.ProjectStatus == ProjectStatus.Expired &&
                project.UpdatedAt < now.AddDays(_options.Value.MaxWorkDeadlineExpirationTimeInDays))
            {
                newStatus = ProjectStatus.Cancelled;

                _logger.LogInformation("Project {ProjectId} marked as Cancelled (Expired deadline)", project.Id);
            }
            else if (now > project.WorkDeadline &&
                     project.ProjectStatus != ProjectStatus.PendingForReview)
            {
                newStatus = ProjectStatus.Expired;

                _logger.LogInformation("Project {ProjectId} marked as Expired (Passed work deadline)", project.Id);
            }
            else if (now > project.WorkStartDate && isProjectHasAcceptedFreelancerApplications)
            {
                newStatus = ProjectStatus.InProgress;

                _logger.LogInformation("Project {ProjectId} marked as InProgress (Work started)", project.Id);

                await _unitOfWork.FreelancerApplicationsRepository.UpdateRejectedStatusWhenNotAcceptedAsync(
                    project.Id, cancellationToken);

                await _unitOfWork.ProjectsRepository.UpdateFreelancerUserIdAsync(project.Id, cancellationToken);

                await _unitOfWork.FreelancerApplicationsRepository.UpdateRejectedStatusWhenNotAcceptedAsync(
                    project.Id, cancellationToken);

                await _unitOfWork.ProjectsRepository.UpdateFreelancerUserIdAsync(project.Id, cancellationToken);
            }
            else if (now > project.WorkStartDate && project.FreelancerUserId is null)
            {
                newStatus = ProjectStatus.Cancelled;

                _logger.LogInformation("Project {ProjectId} marked as Cancelled (No freelancer assigned)", project.Id);
            }
            else if (now > project.ApplicationsDeadline)
            {
                newStatus = ProjectStatus.WaitingForWorkStart;

                _logger.LogInformation(
                    "Project {ProjectId} marked as WaitingForWorkStart (Applications deadline passed)", project.Id);
            }
            else if (now > project.ApplicationsStartDate)
            {
                newStatus = ProjectStatus.AcceptingApplications;

                _logger.LogInformation(
                    "Project {ProjectId} marked as AcceptingApplications (Applications period started)", project.Id);
            }

            if (previousStatus != newStatus)
            {
                _logger.LogInformation("Status changed for project {ProjectId}: {PreviousStatus} -> {NewStatus}",
                    project.Id, previousStatus, project.ProjectStatus);

                await _unitOfWork.LifecyclesRepository.UpdateStatusByProjectIdAsync(
                    project.Id, newStatus, now, cancellationToken);
            }

            // if (lifecycle.ProjectStatus == ProjectStatus.InProgress)
            // {
            //     logger.LogInformation("Updating project {ProjectId} details for InProgress status", project.Id);
            //
            //     await unitOfWork.ProjectCommandsRepository.UpdateAsync(project, cancellationToken);
            // }
            // todo: ???
        }
    }

    private async Task<bool> IsProjectHasAcceptedFreelancerApplications(
        Guid projectId,
        CancellationToken cancellationToken)
    {
        return await _unitOfWork.FreelancerApplicationsRepository
            .AnyByProjectIdAndApplicationStatus(projectId, ApplicationStatus.Accepted, cancellationToken);
    }
}