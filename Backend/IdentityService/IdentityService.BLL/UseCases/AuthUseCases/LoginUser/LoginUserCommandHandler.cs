using IdentityService.BLL.Abstractions.TokenProvider;
using IdentityService.BLL.DTOs;
using IdentityService.BLL.Settings;
using IdentityService.DAL.Abstractions.PasswordHasher;

namespace IdentityService.BLL.UseCases.AuthUseCases.LoginUser;

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, AuthTokensDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenProvider _tokenService;
    private readonly IOptions<JwtSettings> _options;
    private readonly ILogger<LoginUserCommandHandler> _logger;
    private readonly IPasswordHasher _passwordHasher;

    public LoginUserCommandHandler(
        IUnitOfWork unitOfWork,
        ITokenProvider tokenService,
        IOptions<JwtSettings> options,
        ILogger<LoginUserCommandHandler> logger,
        IPasswordHasher passwordHasher)
    {
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _options = options;
        _logger = logger;
        _passwordHasher = passwordHasher;
    }

    public async Task<AuthTokensDto> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.UsersRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user is null)
        {
            _logger.LogError("Invalid login attempt for email {Email}", request.Email);
            throw new UnauthorizedException("Invalid credentials.");
        }

        var isPasswordCorrect = _passwordHasher.VerifyPassword(request.Password, user.PasswordHash);

        if (!isPasswordCorrect)
        {
            _logger.LogError("Invalid password for user {UserId}", user.Id);
            throw new UnauthorizedException("Invalid credentials.");
        }

        if (!user.IsEmailConfirmed)
        {
            _logger.LogError("Login attempt for unconfirmed email {Email}", request.Email);
            throw new UnauthorizedException("You need to confirm your email.");
        }

        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();

        var expiryDays = _options.Value.RefreshTokenExpiryDays;
        var refreshTokenExpiryTime = DateTime.UtcNow.AddDays(expiryDays);

        await _unitOfWork.UsersRepository.UpdateRefreshTokenInfoAsync(user.Id, refreshToken, refreshTokenExpiryTime, cancellationToken);

        return new AuthTokensDto(accessToken, refreshToken);
    }
}