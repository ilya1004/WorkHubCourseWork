using IdentityService.DAL.Constants;
using IdentityService.DAL.Views;

namespace IdentityService.DAL.Repositories;

public class UsersRepository : IUsersRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<UsersRepository> _logger;

    public UsersRepository(ApplicationDbContext context, ILogger<UsersRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<User?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default,
        bool includeRelatedEntities = false)
    {
        try
        {
            var user = await _context.Database
                .SqlQuery<User>($"""
                                 SELECT * FROM "Users" WHERE "Id" = {id.ToString()}
                                 """)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            if (!includeRelatedEntities || user is null)
            {
                return user;
            }

            var role = await _context.Roles
                .FromSqlInterpolated($"""
                          SELECT * FROM "Roles" WHERE "Id" = {user.RoleId.ToString()}
                          """)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            if (role?.Name == AppRoles.FreelancerRole)
            {
                user.FreelancerProfile = await _context.FreelancerProfiles
                    .FromSqlInterpolated($"""
                              SELECT * FROM "FreelancerProfiles" WHERE "UserId" = {user.Id.ToString()}
                              """)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(cancellationToken);
            }

            if (role?.Name == AppRoles.FreelancerRole)
            {
                user.EmployerProfile = await _context.EmployerProfiles
                    .FromSqlInterpolated($"""
                              SELECT * FROM "EmployerProfiles" WHERE "UserId" = {user.Id.ToString()}
                              """)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(cancellationToken);

                if (user.EmployerProfile is not null)
                {
                    user.EmployerProfile.Industry = await _context.EmployerIndustries
                        .FromSqlInterpolated($"""
                                  SELECT * FROM "EmployerIndustries" WHERE "Id" = {user.EmployerProfile.IndustryId.ToString()}
                                  """)
                        .AsNoTracking()
                        .FirstOrDefaultAsync(cancellationToken);
                }
            }

            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to get user by id. Error: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to get user by id. Error: {ex.Message}");
        }
    }

    public async Task<FreelancerUserModel?> GetFreelancerByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _context.Database
                .SqlQuery<FreelancerUserModel>(
                    $"""
                        SELECT * FROM "FreelancerUser" WHERE "Id" = {id.ToString()}
                     """)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to get employer user by id. Error: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to get employer user by id. Error: {ex.Message}");
        }
    }

    public async Task<EmployerUserModel?> GetEmployerByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _context.Database
                .SqlQuery<EmployerUserModel>(
                    $"""
                        SELECT * FROM "EmployerUser" WHERE "Id" = {id.ToString()}
                     """)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to get employer user by id. Error: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to get employer user by id. Error: {ex.Message}");
        }
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _context.Users
                .FromSqlInterpolated($"""
                          SELECT * FROM "Users" WHERE "Email" = {email}
                          """)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            if (user is not null)
            {
                user.Role = await _context.Roles
                    .FromSqlInterpolated($"""
                                          SELECT * FROM "Roles" WHERE "Id" = {user.RoleId}
                                          """)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(cancellationToken);
            }

            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to get user by email. Error: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to get user by email. Error: {ex.Message}");
        }
    }

    public async Task UpdateRefreshTokenInfoAsync(
        Guid id,
        string? refreshToken,
        DateTime? refreshTokenExpiryTime,
        CancellationToken cancellationToken = default)
    {
        try
        {
            int rowsAffected;
            if (refreshToken is not null && refreshTokenExpiryTime is not null)
            {
                rowsAffected = await _context.Database.ExecuteSqlInterpolatedAsync(
                    $"""
                     UPDATE "Users"
                     SET "RefreshToken" = {refreshToken},
                         "RefreshTokenExpiryTime" = {refreshTokenExpiryTime}
                     WHERE "Id" = {id}
                     """,
                    cancellationToken);
            }
            else
            {
                rowsAffected = await _context.Database.ExecuteSqlInterpolatedAsync(
                    $"""
                     UPDATE "Users"
                     SET "RefreshToken" = NULL,
                         "RefreshTokenExpiryTime" = NULL
                     WHERE "Id" = {id}
                     """,
                    cancellationToken);
            }

            if (rowsAffected != 1)
            {
                _logger.LogError("Failed to update user tokens. Affected [{rowsAffected}] rows", rowsAffected);
                throw new InvalidOperationException($"Failed to update user tokens. Affected [{rowsAffected}] rows");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to update user tokens. Error: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to update user tokens. Error: {ex.Message}");
        }
    }

    public async Task<IReadOnlyList<User>> GetAllPaginatedAsync(
        int offset,
        int limit,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Users
                .FromSqlInterpolated($"""
                          SELECT * FROM "User"
                          ORDER BY "Id"
                          LIMIT {limit} OFFSET {offset}
                          """)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to get paginated users. Error: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to get paginated users. Error: {ex.Message}");
        }
    }

    public async Task<int> CountByIsActiveAsync(bool isActive, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Database
                .SqlQuery<int>(
                    $"""
                        SELECT COUNT(*) FROM "User"
                        WHERE "IsActive" = {isActive.ToString()}
                     """)
                .FirstOrDefaultAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to get users count by is active status. Error: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to get users count by is active status. Error: {ex.Message}");
        }
    }

    public async Task<int> CountAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Database
                .SqlQuery<int>(
                    $"""
                        SELECT COUNT(*) FROM "User"
                     """)
                .FirstOrDefaultAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to get users count. Error: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to get users count. Error: {ex.Message}");
        }
    }

    public async Task UpdateIsEmailConfirmedAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var rowsAffected = await _context.Database.ExecuteSqlInterpolatedAsync(
                $"""
                 UPDATE "Users"
                 SET "IsEmailConfirmed" = TRUE
                 WHERE "Id" = {id.ToString()}
                 """,
                cancellationToken);

            if (rowsAffected != 1)
            {
                _logger.LogError("Failed to confirm user email. Affected [{rowsAffected}] rows", rowsAffected);
                throw new InvalidOperationException($"Failed to confirm user email. Affected [{rowsAffected}] rows");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to confirm user email. Error: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to confirm user email. Error: {ex.Message}");
        }
    }

    public async Task UpdatePasswordHashAsync(Guid id, string passwordHash, CancellationToken cancellationToken = default)
    {
        try
        {
            var rowsAffected = await _context.Database.ExecuteSqlInterpolatedAsync(
                $"""
                 UPDATE "Users"
                 SET "PasswordHash" = {passwordHash}
                 WHERE "Id" = {id.ToString()}
                 """,
                cancellationToken);

            if (rowsAffected != 1)
            {
                _logger.LogError("Failed to update password hash. Affected [{rowsAffected}] rows", rowsAffected);
                throw new InvalidOperationException($"Failed to update password hash. Affected [{rowsAffected}] rows");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to update password hash. Error: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to update password hash. Error: {ex.Message}");
        }
    }

    public async Task UpdateUserImageAsync(Guid id, string? imageUrl, CancellationToken cancellationToken = default)
    {
        try
        {
            var rowsAffected = await _context.Database.ExecuteSqlInterpolatedAsync(
                $"""
                 UPDATE "Users"
                 SET "ImageUrl" = {imageUrl}
                 WHERE "Id" = {id.ToString()}
                 """,
                cancellationToken);

            if (rowsAffected != 1)
            {
                _logger.LogError("Failed to update image url. Affected [{rowsAffected}] rows", rowsAffected);
                throw new InvalidOperationException($"Failed to update image url. Affected [{rowsAffected}] rows");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to update image url. Error: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to update image url. Error: {ex.Message}");
        }
    }

    public async Task CreateAsync(User user, CancellationToken cancellationToken = default)
    {
        try
        {
            var rowsAffected = await _context.Database.ExecuteSqlInterpolatedAsync(
                $"""
                 INSERT INTO "Users" ("Id", "RegisteredAt", "Email", "PasswordHash", "IsEmailConfirmed", "RoleId")
                 VALUES ({user.Id}, {user.RegisteredAt}, {user.Email}, {user.PasswordHash}, {user.IsEmailConfirmed}, {user.RoleId})
                 """,
                cancellationToken);

            if (rowsAffected != 1)
            {
                _logger.LogError("Failed to create user. Affected [{rowsAffected}] rows", rowsAffected);
                throw new InvalidOperationException($"Failed to create user. Affected [{rowsAffected}] rows");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to create user. Error: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to create user. Error: {ex.Message}");
        }
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var rowsAffected = await _context.Database.ExecuteSqlInterpolatedAsync(
                $"""
                 DELETE FROM "Users"
                 WHERE "Id" = {id}
                 """,
                cancellationToken);

            if (rowsAffected != 1)
            {
                _logger.LogError("Failed to delete user. Affected [{rowsAffected}] rows", rowsAffected);
                throw new InvalidOperationException($"Failed to delete user. Affected [{rowsAffected}] rows");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to delete user. Error: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to delete user. Error: {ex.Message}");
        }
    }
}