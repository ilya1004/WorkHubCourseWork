using ProjectsService.Domain.Abstractions.KafkaProducerServices;
using ProjectsService.Domain.Abstractions.UserContext;

namespace ProjectsService.Application.UseCases.Commands.ProjectUseCases.CancelProject;

public class CancelProjectCommandHandler : IRequestHandler<CancelProjectCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    private readonly IPaymentsProducerService _paymentsProducerService;
    private readonly ILogger<CancelProjectCommandHandler> _logger;

    public CancelProjectCommandHandler(
        IUnitOfWork unitOfWork,
        IUserContext userContext,
        IPaymentsProducerService paymentsProducerService,
        ILogger<CancelProjectCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
        _paymentsProducerService = paymentsProducerService;
        _logger = logger;
    }

    public async Task Handle(CancelProjectCommand request, CancellationToken cancellationToken)
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
            _logger.LogError("User {UserId} attempted to cancel project {ProjectId} without permission",
                userId, request.ProjectId);
            throw new ForbiddenException($"You do not have access to project with ID '{request.ProjectId}'");
        }

        var lifecycle = project.Lifecycle;

        lifecycle.ProjectStatus = ProjectStatus.Cancelled;

        await _unitOfWork.LifecyclesRepository.UpdateAsync(lifecycle, cancellationToken);

        if (project.PaymentIntentId is not null)
        {
            await _paymentsProducerService.CancelPaymentAsync(project.PaymentIntentId, cancellationToken);
        }
    }
}