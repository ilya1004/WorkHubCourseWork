using IdentityService.BLL.Abstractions.UserContext;

namespace IdentityService.BLL.UseCases.AuthUseCases.LogoutUser;

public class LogoutUserCommandHandler : IRequestHandler<LogoutUserCommand>
{
    private readonly IUserContext _userContext;
    private readonly ILogger<LogoutUserCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public LogoutUserCommandHandler(
        IUserContext userContext,
        ILogger<LogoutUserCommandHandler> logger,
        IUnitOfWork unitOfWork)
    {
        _userContext = userContext;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(LogoutUserCommand request, CancellationToken cancellationToken)
    {
        var userId = _userContext.GetUserId();

        var user = await _unitOfWork.UsersRepository.GetByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            _logger.LogError("User with ID '{UserId}' not found during logout", userId);
            throw new NotFoundException($"User with ID '{userId}' not found");
        }

        if (user.RefreshToken is null && user.RefreshTokenExpiryTime is null)
        {
            _logger.LogWarning("User with ID '{UserId}' has already logout", userId);
            return;
        }

        await _unitOfWork.UsersRepository.UpdateRefreshTokenInfoAsync(user.Id, null, null, cancellationToken);
    }
}