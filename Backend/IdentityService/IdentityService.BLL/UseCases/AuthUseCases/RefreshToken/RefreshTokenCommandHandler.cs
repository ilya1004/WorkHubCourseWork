using IdentityService.BLL.DTOs;
using IdentityService.BLL.Settings;
using System.Security.Claims;
using IdentityService.BLL.Abstractions.TokenProvider;

namespace IdentityService.BLL.UseCases.AuthUseCases.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthTokensDto>
{
    private readonly ITokenProvider _tokenProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOptions<JwtSettings> _options;
    private readonly ILogger<RefreshTokenCommandHandler> _logger;

    public RefreshTokenCommandHandler(
        ITokenProvider tokenProvider,
        IUnitOfWork unitOfWork,
        IOptions<JwtSettings> options,
        ILogger<RefreshTokenCommandHandler> logger)
    {
        _tokenProvider = tokenProvider;
        _unitOfWork = unitOfWork;
        _options = options;
        _logger = logger;
    }

    public async Task<AuthTokensDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var principal = _tokenProvider.GetPrincipalFromExpiredToken(request.AccessToken);
        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);

        var user = await _unitOfWork.UsersRepository.GetByIdAsync(Guid.Parse(userId!), cancellationToken);

        if (user is null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime < DateTime.UtcNow)
        {
            _logger.LogError("Invalid refresh token for user {UserId}", userId);
            throw new UnauthorizedException("Invalid refresh token");
        }

        var newAccessToken = _tokenProvider.GenerateAccessToken(user);
        var newRefreshToken = _tokenProvider.GenerateRefreshToken();

        var expiryDays = _options.Value.RefreshTokenExpiryDays;
        var refreshTokenExpiryTime = DateTime.UtcNow.AddDays(expiryDays);

        await _unitOfWork.UsersRepository.UpdateRefreshTokenInfoAsync(user.Id, newRefreshToken, refreshTokenExpiryTime, cancellationToken);

        return new AuthTokensDto(newAccessToken, newRefreshToken);
    }
}