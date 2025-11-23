using IdentityService.BLL.Abstractions.BlobService;
using IdentityService.BLL.Abstractions.UserContext;
using IdentityService.DAL.Constants;

namespace IdentityService.BLL.UseCases.UserUseCases.Commands.DeleteUserCommand;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBlobService _blobService;
    private readonly ILogger<DeleteUserCommandHandler> _logger;
    private readonly IUserContext _userContext;

    public DeleteUserCommandHandler(IUnitOfWork unitOfWork,
        IBlobService blobService,
        ILogger<DeleteUserCommandHandler> logger,
        IUserContext userContext)
    {
        _unitOfWork = unitOfWork;
        _blobService = blobService;
        _logger = logger;
        _userContext = userContext;
    }

    public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _userContext.GetUserId();
        var currentUserRoleName = _userContext.GetRoleName();

        if (currentUserId != request.UserId || currentUserRoleName != AppRoles.AdminRole)
        {
            _logger.LogError("You don't have access to user with ID {UserId}", request.UserId);
            throw new NotFoundException($"You don't have access to user with ID {request.UserId}");
        }

        var user = await _unitOfWork.UsersRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            _logger.LogError("User with ID {UserId} not found", request.UserId);
            throw new NotFoundException($"User with ID '{request.UserId}' not found");
        }

        if (!string.IsNullOrEmpty(user.ImageUrl))
        {
            await _blobService.DeleteAsync(Guid.Parse(user.ImageUrl), cancellationToken);
        }

        await _unitOfWork.UsersRepository.DeleteAsync(user.Id, cancellationToken);
    }
}