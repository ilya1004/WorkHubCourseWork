using IdentityService.DAL.Abstractions.PasswordHasher;

namespace IdentityService.BLL.UseCases.UserUseCases.Commands.ChangePassword;

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand>
{
    private readonly ILogger<ChangePasswordCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;

    public ChangePasswordCommandHandler(
        ILogger<ChangePasswordCommandHandler> logger,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public async Task Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.UsersRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user is null)
        {
            _logger.LogError("User with email {Email} not found", request.Email);
            throw new NotFoundException($"User with email '{request.Email}' not found");
        }

        var isPasswordCorrect = _passwordHasher.VerifyPassword(request.CurrentPassword, user.PasswordHash);

        if (!isPasswordCorrect)
        {
            _logger.LogError("Invalid password for user {UserId}", user.Id);
            throw new UnauthorizedException("Invalid credentials.");
        }

        var newPasswordHash = _passwordHasher.HashPassword(request.NewPassword);

        await _unitOfWork.UsersRepository.UpdatePasswordHashAsync(user.Id, newPasswordHash, cancellationToken);
    }
}