using IdentityService.DAL.Views;

namespace IdentityService.DAL.Abstractions.Repositories;

public interface IUsersRepository
{
    Task<User?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default,
        bool includeRelatedEntities = false);
    Task<FreelancerUserModel?> GetFreelancerByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<EmployerUserModel?> GetEmployerByIdAsync(Guid id, CancellationToken cancellationToken = default)
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<User>> GetAllPaginatedAsync(
        int offset,
        int limit,
        CancellationToken cancellationToken = default);
    Task<int> CountByIsActiveAsync(bool isActive, CancellationToken cancellationToken = default);
    Task<int> CountAllAsync(CancellationToken cancellationToken = default);
    Task UpdateIsEmailConfirmedAsync(Guid id, CancellationToken cancellationToken = default);
    Task UpdateRefreshTokenInfoAsync(
        Guid id,
        string? refreshToken,
        DateTime? refreshTokenExpiryTime,
        CancellationToken cancellationToken = default);
    Task UpdatePasswordHashAsync(Guid id, string passwordHash, CancellationToken cancellationToken = default);
    Task UpdateUserAsync(User user, CancellationToken cancellationToken = default);
    Task UpdateUserImageAsync(Guid id, string? imageUrl, CancellationToken cancellationToken = default);
    Task CreateAsync(User user, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}