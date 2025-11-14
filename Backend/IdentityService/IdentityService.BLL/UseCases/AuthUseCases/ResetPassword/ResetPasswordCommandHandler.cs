using IdentityService.BLL.Abstractions.TokenProvider;
using IdentityService.DAL.Abstractions.PasswordHasher;
using IdentityService.DAL.Abstractions.RedisService;

namespace IdentityService.BLL.UseCases.AuthUseCases.ResetPassword;

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand>
{
    private readonly ICachedService _cachedService;
    private readonly ILogger<ResetPasswordCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenProvider _tokenProvider;
    private readonly IPasswordHasher _passwordHasher;

    public ResetPasswordCommandHandler(
        ICachedService cachedService,
        ILogger<ResetPasswordCommandHandler> logger,
        IUnitOfWork unitOfWork,
        ITokenProvider tokenProvider,
        IPasswordHasher passwordHasher)
    {
        _cachedService = cachedService;
        _logger = logger;
        _unitOfWork = unitOfWork;
        _tokenProvider = tokenProvider;
        _passwordHasher = passwordHasher;
    }

    public async Task Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.UsersRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user is null)
        {
            _logger.LogError("User with email {Email} not found", request.Email);
            throw new NotFoundException("User with this email does not exist.");
        }

        var token = await _cachedService.GetAsync(request.Code, cancellationToken);
        
        if (string.IsNullOrEmpty(token))
        {
            _logger.LogError("Invalid reset code {Code}", request.Code);
            throw new BadRequestException("Invalid resetting password code.");
        }

        var isTokenValid = _tokenProvider.VerifyPasswordResetToken(user, token);

        if (!isTokenValid)
        {
            _logger.LogError("Invalid password reset token.");
            throw new BadRequestException("Invalid password reset token.");
        }

        var passwordHash = _passwordHasher.HashPassword(request.NewPassword);

        await _unitOfWork.UsersRepository.UpdatePasswordHashAsync(user.Id, passwordHash, cancellationToken);

        await _cachedService.DeleteAsync(request.Code, cancellationToken);
    }
}