using IdentityService.BLL.Settings;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using IdentityService.BLL.Abstractions.TokenProvider;

namespace IdentityService.BLL.Services.TokenProvider;

public class TokenProvider : ITokenProvider
{
    private readonly IOptions<JwtSettings> _options;
    private readonly ILogger<TokenProvider> _logger;

    public TokenProvider(IOptions<JwtSettings> options,
        ILogger<TokenProvider> logger)
    {
        _options = options;
        _logger = logger;
    }

    public string GenerateAccessToken(User user)
    {
        _logger.LogInformation("Generating access token for user {UserId}", user.Id);
        
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.Name)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Value.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            _options.Value.Issuer,
            _options.Value.Audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(_options.Value.AccessTokenExpiryMinutes),
            signingCredentials: credentials);

        _logger.LogInformation("Access token generated successfully for user {UserId}", user.Id);
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        _logger.LogInformation("Generating refresh token");
        
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();

        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        _logger.LogInformation("Validating expired token");
        
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _options.Value.Issuer,
            ValidAudience = _options.Value.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Value.SecretKey)),
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
        var jwtSecurityToken = securityToken as JwtSecurityToken;

        if (jwtSecurityToken is null ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            _logger.LogWarning("Invalid token received");
            
            throw new UnauthorizedException("Invalid token");
        }

        _logger.LogInformation("Token validated successfully");
        
        return principal;
    }

    public string GeneratePasswordResetToken(User user)
    {
        var idString = Convert.ToBase64String(Encoding.UTF8.GetBytes(user.Id.ToString()));
        var emailString = Convert.ToBase64String(Encoding.UTF8.GetBytes(user.Email));
        var roleString = Convert.ToBase64String(Encoding.UTF8.GetBytes(user.RoleId.ToString()));
        return string.Concat(idString, emailString, roleString);
    }

    public bool VerifyPasswordResetToken(User user, string token)
    {
        var idString = Convert.ToBase64String(Encoding.UTF8.GetBytes(user.Id.ToString()));
        var emailString = Convert.ToBase64String(Encoding.UTF8.GetBytes(user.Email));
        var roleString = Convert.ToBase64String(Encoding.UTF8.GetBytes(user.RoleId.ToString()));
        return token == string.Concat(idString, emailString, roleString);
    }

    public string GenerateEmailResetToken(User user)
    {
        var idString = Convert.ToBase64String(Encoding.UTF8.GetBytes(user.Id.ToString()));
        var emailString = Convert.ToBase64String(Encoding.UTF8.GetBytes(user.Email));
        var roleString = Convert.ToBase64String(Encoding.UTF8.GetBytes(user.RoleId.ToString()));
        return string.Concat(idString, emailString, roleString);
    }

    public bool VerifyEmailResetToken(User user, string token)
    {
        var idString = Convert.ToBase64String(Encoding.UTF8.GetBytes(user.Id.ToString()));
        var emailString = Convert.ToBase64String(Encoding.UTF8.GetBytes(user.Email));
        var roleString = Convert.ToBase64String(Encoding.UTF8.GetBytes(user.RoleId.ToString()));
        return token == string.Concat(idString, emailString, roleString);
    }
}