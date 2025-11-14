using Microsoft.Extensions.Options;
using ProjectsService.Application.Settings;

namespace ProjectsService.Application.BackgroundJobs.UpdateProjectStatuses;

public class UpdateProjectStatusesCommandHandler(
    IUnitOfWork unitOfWork,
    IOptions<ProjectsSettings> options,
    ILogger<UpdateProjectStatusesCommandHandler> logger) : IRequestHandler<UpdateProjectStatusesCommand>
{
    public async Task Handle(UpdateProjectStatusesCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting update of project statuses");

        var projects = await unitOfWork.ProjectQueriesRepository.ListAsync(
            p => 
                p.Lifecycle.ProjectStatus != ProjectStatus.Completed && 
                p.Lifecycle.ProjectStatus != ProjectStatus.Cancelled, 
            cancellationToken, 
            p => p.Lifecycle,
            p => p.FreelancerApplications);

        logger.LogInformation("Found {ProjectCount} projects to process", projects.Count);

        foreach (var project in projects)
        {
            var lifecycle = project.Lifecycle;
            var previousStatus = lifecycle.ProjectStatus;
            var now = DateTime.UtcNow;
            
            logger.LogInformation("Processing project {ProjectId}, current status: {CurrentStatus}", 
                project.Id, previousStatus);

            if (lifecycle.AcceptanceStatus == ProjectAcceptanceStatus.Accepted)
            {
                lifecycle.ProjectStatus = ProjectStatus.Completed;
                
                logger.LogInformation("Project {ProjectId} marked as Completed (AcceptanceConfirmed)", project.Id);
            }
            else if (now > lifecycle.WorkDeadline && 
                lifecycle.ProjectStatus == ProjectStatus.Expired &&
                lifecycle.UpdatedAt < now.AddDays(options.Value.MaxWorkDeadlineExpirationTimeInDays))
            {
                lifecycle.ProjectStatus = ProjectStatus.Cancelled;
                
                logger.LogInformation("Project {ProjectId} marked as Cancelled (Expired deadline)", project.Id);
            }
            else if (now > lifecycle.WorkDeadline && 
                     lifecycle.ProjectStatus != ProjectStatus.PendingForReview)
            {
                lifecycle.ProjectStatus = ProjectStatus.Expired;
                
                logger.LogInformation("Project {ProjectId} marked as Expired (Passed work deadline)", project.Id);
            }
            else if (now > lifecycle.WorkStartDate &&
                     IsProjectHasAcceptedFreelancerApplications(project))
            {
                lifecycle.ProjectStatus = ProjectStatus.InProgress;
                
                logger.LogInformation("Project {ProjectId} marked as InProgress (Work started)", project.Id);
                
                UpdateProjectWhenInProgressAsync(project);
            }
            else if (now > lifecycle.WorkStartDate && project.FreelancerUserId is null)
            {
                lifecycle.ProjectStatus = ProjectStatus.Cancelled;
                
                logger.LogInformation("Project {ProjectId} marked as Cancelled (No freelancer assigned)", project.Id);
            }
            else if (now > lifecycle.ApplicationsDeadline)
            {
                lifecycle.ProjectStatus = ProjectStatus.WaitingForWorkStart;
                
                logger.LogInformation("Project {ProjectId} marked as WaitingForWorkStart (Applications deadline passed)", project.Id);
            }
            else if (now > lifecycle.ApplicationsStartDate)
            {
                lifecycle.ProjectStatus = ProjectStatus.AcceptingApplications;
                
                logger.LogInformation("Project {ProjectId} marked as AcceptingApplications (Applications period started)", project.Id);
            }
            
            if (previousStatus != lifecycle.ProjectStatus)
            {
                logger.LogInformation("Status changed for project {ProjectId}: {PreviousStatus} -> {NewStatus}", 
                    project.Id, previousStatus, lifecycle.ProjectStatus);
                
                lifecycle.UpdatedAt = now;
                await unitOfWork.LifecycleCommandsRepository.UpdateAsync(lifecycle, cancellationToken);
            }
            
            if (lifecycle.ProjectStatus == ProjectStatus.InProgress)
            {
                logger.LogInformation("Updating project {ProjectId} details for InProgress status", project.Id);
                
                await unitOfWork.ProjectCommandsRepository.UpdateAsync(project, cancellationToken);
            }
        }

        logger.LogInformation("Saving all changes for {ProjectCount} projects", projects.Count);
        
        await unitOfWork.SaveAllAsync(cancellationToken);
        
        logger.LogInformation("Project statuses update completed");
    }

    private static bool IsProjectHasAcceptedFreelancerApplications(Project project)
    {
        return project.FreelancerApplications.Any(a => a.Status == ApplicationStatus.Accepted);
    }

    private static void UpdateProjectWhenInProgressAsync(Project project)
    {
        foreach (var application in project.FreelancerApplications)
        {
            if (application.Status != ApplicationStatus.Accepted)
            {
                application.Status = ApplicationStatus.Rejected;
            }
            else
            {
                project.FreelancerUserId = application.FreelancerUserId; 
            }
        }
    }
}