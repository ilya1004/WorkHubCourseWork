using System.Linq.Expressions;

namespace IdentityService.DAL.Abstractions.Repositories;

public interface IUsersRepository
{
    // Task<User?> GetByIdAsync(
    //     Guid id,
    //     bool withTracking = true,
    //     CancellationToken cancellationToken = default,
    //     params Expression<Func<User, object>>[]? includesProperties);
    //
    // Task<User?> FirstOrDefaultAsync(
    //     Expression<Func<User, bool>> filter,
    //     CancellationToken cancellationToken = default,
    //     params Expression<Func<User, object>>[]? includesProperties);
    //
    // Task<IReadOnlyList<User>> PaginatedListAllAsync(
    //     int offset,
    //     int limit,
    //     CancellationToken cancellationToken = default,
    //     params Expression<Func<User, object>>[]? includesProperties);
    //
    // Task<IReadOnlyList<User>> PaginatedListAsync(
    //     Expression<Func<User, bool>>? filter,
    //     int offset,
    //     int limit,
    //     CancellationToken cancellationToken = default,
    //     params Expression<Func<User, object>>[]? includesProperties);
    //
    // Task UpdateAsync(User entity, CancellationToken cancellationToken = default);
    // Task DeleteAsync(User entity, CancellationToken cancellationToken = default);
    // Task<int> CountAsync(Expression<Func<User, bool>>? filter, CancellationToken cancellationToken = default);

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
}