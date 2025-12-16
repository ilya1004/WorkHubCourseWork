using IdentityService.BLL.Abstractions.UserContext;
using IdentityService.DAL.Constants;

namespace IdentityService.BLL.UseCases.CvUseCases.Queries.GetCvById;

public class GetCvByIdQueryHandler : IRequestHandler<GetCvByIdQuery, Cv>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetCvByIdQueryHandler> _logger;
    private readonly IUserContext _userContext;

    public GetCvByIdQueryHandler(
        IUnitOfWork unitOfWork,
        ILogger<GetCvByIdQueryHandler> logger,
        IUserContext userContext)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _userContext = userContext;
    }

    public async Task<Cv> Handle(GetCvByIdQuery request, CancellationToken cancellationToken)
    {
        var cv = await _unitOfWork.CvsRepository.GetByIdAsync(request.Id, cancellationToken, true);

        if (cv is null)
        {
            _logger.LogError("Cv with ID {IndustryId} not found", request.Id);
            throw new NotFoundException($"Cv with ID '{request.Id}' not found");
        }

        var userId = _userContext.GetUserId();
        var isUserHasAccessToPrivate = userId == cv.FreelancerUserId && !cv.IsPublic;
        var isAdmin = _userContext.GetRoleName() == AppRoles.AdminRole;

        if (!isUserHasAccessToPrivate && !isAdmin)
        {
            _logger.LogError("User {UserId} attempted to get cv {CvId} without permission",
                userId, request.Id);
            throw new ForbiddenException($"You do not have access to cv with ID '{request.Id}'");
        }

        return cv;
    }
}