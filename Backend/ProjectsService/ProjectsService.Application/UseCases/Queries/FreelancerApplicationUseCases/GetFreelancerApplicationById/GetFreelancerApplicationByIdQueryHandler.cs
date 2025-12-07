using ProjectsService.Application.Constants;
using ProjectsService.Domain.Abstractions.UserContext;

namespace ProjectsService.Application.UseCases.Queries.FreelancerApplicationUseCases.GetFreelancerApplicationById;

public class GetFreelancerApplicationByIdQueryHandler : IRequestHandler<GetFreelancerApplicationByIdQuery, FreelancerApplication>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    private readonly ILogger<GetFreelancerApplicationByIdQueryHandler> _logger;

    public GetFreelancerApplicationByIdQueryHandler(IUnitOfWork unitOfWork,
        IUserContext userContext,
        ILogger<GetFreelancerApplicationByIdQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
        _logger = logger;
    }

    public async Task<FreelancerApplication> Handle(GetFreelancerApplicationByIdQuery request, CancellationToken cancellationToken)
    {
        var application = await _unitOfWork.FreelancerApplicationsRepository.GetByIdAsync(
            request.ApplicationId, cancellationToken, true);
        
        if (application is null)
        {
            _logger.LogError("Freelancer application with ID {ApplicationId} not found", request.ApplicationId);
            throw new NotFoundException($"Freelancer Application with ID '{request.ApplicationId}' not found");
        }

        var userId = _userContext.GetUserId();
        var isResourceOwned = userId == application.FreelancerUserId;
        var isAdmin = _userContext.GetUserRole() == AppRoles.AdminRole;
        
        if (!isResourceOwned && !isAdmin)
        {
            _logger.LogError("User {UserId} attempted to access application {ApplicationId} without permission", userId, request.ApplicationId);
            throw new ForbiddenException($"You do not have access to Freelancer Application with ID '{request.ApplicationId}'");
        }

        return application;
    }
}