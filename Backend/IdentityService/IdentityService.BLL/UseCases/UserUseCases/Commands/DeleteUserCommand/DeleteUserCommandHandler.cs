using IdentityService.BLL.Abstractions.BlobService;

namespace IdentityService.BLL.UseCases.UserUseCases.Commands.DeleteUserCommand;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBlobService _blobService;
    private readonly ILogger<DeleteUserCommandHandler> _logger;

    public DeleteUserCommandHandler(IUnitOfWork unitOfWork,
        IBlobService blobService,
        ILogger<DeleteUserCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _blobService = blobService;
        _logger = logger;
    }

    public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
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