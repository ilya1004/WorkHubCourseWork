using ProjectsService.Domain.Abstractions.UserContext;

namespace ProjectsService.Application.UseCases.Commands.FreelancerApplicationUseCases.CreateFreelancerApplication;

public class CreateFreelancerApplicationCommandHandler : IRequestHandler<CreateFreelancerApplicationCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    private readonly ILogger<CreateFreelancerApplicationCommandHandler> _logger;

    public CreateFreelancerApplicationCommandHandler(
        IUnitOfWork unitOfWork,
        IUserContext userContext,
        ILogger<CreateFreelancerApplicationCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
        _logger = logger;
    }

    public async Task Handle(CreateFreelancerApplicationCommand request, CancellationToken cancellationToken)
    {
        var userId = _userContext.GetUserId();

        var freelancerApplications =
            await _unitOfWork.FreelancerApplicationsRepository.GetByProjectIdAsync(
                request.ProjectId, cancellationToken);

        var userFreelancerApplication = freelancerApplications
            .FirstOrDefault(x => x.FreelancerUserId == userId);

        if (userFreelancerApplication is not null)
        {
            _logger.LogError("User {UserId} already has application for project {ProjectId}", userId,
                request.ProjectId);
            throw new AlreadyExistsException(
                $"Freelancer application to the project with ID '{request.ProjectId}' already exists.");
        }

        var newFreelancerApplication = new FreelancerApplication
        {
            Id = Guid.CreateVersion7(),
            ProjectId = request.ProjectId,
            CreatedAt = DateTime.UtcNow,
            CvId = request.CvId,
            FreelancerUserId = userId,
            Status = ApplicationStatus.Pending,
        };

        await _unitOfWork.FreelancerApplicationsRepository.CreateAsync(newFreelancerApplication, cancellationToken);
    }
}