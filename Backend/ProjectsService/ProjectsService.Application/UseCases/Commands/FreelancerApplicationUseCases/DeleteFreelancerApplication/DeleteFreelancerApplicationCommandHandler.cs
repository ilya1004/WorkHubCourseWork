using ProjectsService.Application.Constants;
using ProjectsService.Domain.Abstractions.UserContext;

namespace ProjectsService.Application.UseCases.Commands.FreelancerApplicationUseCases.DeleteFreelancerApplication;

public class DeleteFreelancerApplicationCommandHandler : IRequestHandler<DeleteFreelancerApplicationCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    private readonly ILogger<DeleteFreelancerApplicationCommandHandler> _logger;

    public DeleteFreelancerApplicationCommandHandler(IUnitOfWork unitOfWork,
        IUserContext userContext,
        ILogger<DeleteFreelancerApplicationCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
        _logger = logger;
    }

    public async Task Handle(DeleteFreelancerApplicationCommand request, CancellationToken cancellationToken)
    {
        var freelancerApplication = await _unitOfWork.FreelancerApplicationsRepository.GetByIdAsync(
            request.ApplicationId, cancellationToken);
        
        if (freelancerApplication is null)
        {
            _logger.LogError("Freelancer application {ApplicationId} not found", request.ApplicationId);
            throw new NotFoundException($"Freelancer Application with ID '{request.ApplicationId}' not found");
        }
        
        var userId = _userContext.GetUserId();
        var isResourceOwned = userId == freelancerApplication.FreelancerUserId;
        var isAdmin = _userContext.GetUserRole() == AppRoles.AdminRole;

        if (!isResourceOwned && !isAdmin)
        {
            _logger.LogError("User {UserId} attempted to delete application {ApplicationId} without permission",
                userId, request.ApplicationId);
            throw new ForbiddenException($"You do not have access to Freelancer Application with ID '{request.ApplicationId}'");
        }

        await _unitOfWork.FreelancerApplicationsRepository.DeleteAsync(freelancerApplication.Id, cancellationToken);
    }
}