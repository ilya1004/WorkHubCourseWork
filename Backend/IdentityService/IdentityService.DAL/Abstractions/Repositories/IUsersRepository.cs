namespace IdentityService.DAL.Abstractions.Repositories;

public interface IUsersRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default, bool includeProfile = false);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task UpdateIsEmailConfirmedAsync(Guid id, CancellationToken cancellationToken = default);
    Task UpdateRefreshTokenInfoAsync(
        Guid id,
        string? refreshToken,
        DateTime? refreshTokenExpiryTime,
        CancellationToken cancellationToken = default);
    Task UpdatePasswordHashAsync(Guid id, string passwordHash, CancellationToken cancellationToken = default);
    Task CreateAsync(User user, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}