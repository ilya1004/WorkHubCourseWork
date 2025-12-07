namespace ProjectsService.Application.UseCases.Queries.ProjectUseCases.GetProjectById;

public class GetProjectByIdQueryHandler : IRequestHandler<GetProjectByIdQuery, Project>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetProjectByIdQueryHandler> _logger;

    public GetProjectByIdQueryHandler(
        IUnitOfWork unitOfWork,
        ILogger<GetProjectByIdQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Project> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
    {
        var project = await _unitOfWork.ProjectsRepository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (project is null)
        {
            _logger.LogError("Project with ID {ProjectId} not found", request.Id);
            throw new NotFoundException($"Project with ID '{request.Id}' not found");
        }

        return project;
    }
}