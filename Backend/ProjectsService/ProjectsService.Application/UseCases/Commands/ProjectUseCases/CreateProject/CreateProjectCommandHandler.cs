using ProjectsService.Domain.Abstractions.UserContext;

namespace ProjectsService.Application.UseCases.Commands.ProjectUseCases.CreateProject;

public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, Project>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    private readonly ILogger<CreateProjectCommandHandler> _logger;

    public CreateProjectCommandHandler(
        IUnitOfWork unitOfWork,
        IUserContext userContext,
        ILogger<CreateProjectCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
        _logger = logger;
    }

    public async Task<Project> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        var userId = _userContext.GetUserId();

        var existingProject = await _unitOfWork.ProjectsRepository.GetByEmployerAndTitleAsync(
            userId, request.Project.Title, cancellationToken);

        if (existingProject is not null)
        {
            _logger.LogError("Project with title '{Title}' already exists for user {UserId}", request.Project.Title,
                userId);
            throw new AlreadyExistsException(
                $"Project with title '{request.Project.Title}' already exists with this employer.");
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
        }

        var project = new Project
        {
            Id = Guid.CreateVersion7(),
            Title = request.Project.Title,
            Description = request.Project.Description,
            Budget = request.Project.Budget,
            EmployerUserId = userId,
            IsActive = true,
            CategoryId = request.Project.CategoryId,
        };

        var lifecycle = new Lifecycle
        {
            Id = Guid.CreateVersion7(),
            CreatedAt = DateTime.UtcNow,
            ApplicationsStartDate = request.Lifecycle.ApplicationsStartDate,
            ApplicationsDeadline = request.Lifecycle.ApplicationsDeadline,
            WorkStartDate = default,
            WorkDeadline = default,
            AcceptanceStatus = ProjectAcceptanceStatus.None,
            ProjectStatus = ProjectStatus.Published,
            ProjectId = project.Id,
        };

        await _unitOfWork.ProjectsRepository.CreateAsync(project, cancellationToken);

        await _unitOfWork.LifecyclesRepository.CreateAsync(lifecycle, cancellationToken);

        return project;
    }
}