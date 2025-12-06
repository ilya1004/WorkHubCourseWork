using ProjectsService.Domain.Abstractions.UserContext;

namespace ProjectsService.Application.UseCases.Commands.ProjectUseCases.UpdateProject;

public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand, Project>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    private readonly ILogger<UpdateProjectCommandHandler> _logger;

    public UpdateProjectCommandHandler(
        IUnitOfWork unitOfWork,
        IUserContext userContext,
        ILogger<UpdateProjectCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
        _logger = logger;
    }

    public async Task<Project> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
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
            _logger.LogError("User {UserId} attempted to update project {ProjectId} without permission", userId, request.ProjectId);
            throw new ForbiddenException($"You do not have access to project with ID '{request.ProjectId}'");
        }

        if (project.Lifecycle.ProjectStatus != ProjectStatus.Published)
        {
            _logger.LogError("Invalid project status {Status} for update", project.Lifecycle.ProjectStatus);
            throw new BadRequestException("You cannot edit this project after the start of accepting applications");
        }
        
        if (request.Project.CategoryId.HasValue)
        {
            _logger.LogInformation("Checking category {CategoryId} existence", request.Project.CategoryId);
            
            var existingCategory = await _unitOfWork.CategoriesRepository.GetByIdAsync(
                request.Project.CategoryId.Value, cancellationToken);

            if (existingCategory is null)
            {
                _logger.LogError("Category {CategoryId} not found", request.Project.CategoryId);
                throw new NotFoundException($"Category with ID '{request.Project.CategoryId}' not found");
            }

            project.CategoryId = request.Project.CategoryId.Value;
        }

        project.Description = request.Project.Description;
        project.Budget = request.Project.Budget;

        var lifecycle = project.Lifecycle;

        lifecycle.ApplicationsStartDate = request.Lifecycle.ApplicationsStartDate;
        lifecycle.ApplicationsDeadline = request.Lifecycle.ApplicationsDeadline;
        lifecycle.WorkStartDate = request.Lifecycle.WorkStartDate;
        lifecycle.WorkDeadline = request.Lifecycle.WorkDeadline;
        lifecycle.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.ProjectsRepository.UpdateAsync(project, cancellationToken);
        await _unitOfWork.LifecyclesRepository.UpdateAsync(lifecycle, cancellationToken);

        return project;
    }
}