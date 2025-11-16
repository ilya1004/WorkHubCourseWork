using IdentityService.DAL.Constants;

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

    // public async Task<User?> GetByIdAsync(
    //     Guid id,
    //     bool withTracking = true,
    //     CancellationToken cancellationToken = default,
    //     params Expression<Func<User, object>>[]? includesProperties)
    // {
    //     var query = withTracking ? _context.Users.AsQueryable() : _context.Users.AsQueryable().AsNoTracking();
    //
    //     if (includesProperties != null)
    //     {
    //         foreach (var includeProperty in includesProperties)
    //         {
    //             query = query.Include(includeProperty);
    //         }
    //     }
    //
    //     return await query.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    // }
    //
    // public async Task<User?> FirstOrDefaultAsync(
    //     Expression<Func<User, bool>> filter,
    //     CancellationToken cancellationToken = default,
    //     params Expression<Func<User, object>>[]? includesProperties)
    // {
    //     var query = _context.Users.AsQueryable().AsNoTracking();
    //
    //     if (includesProperties != null)
    //     {
    //         foreach (var includeProperty in includesProperties)
    //         {
    //             query = query.Include(includeProperty);
    //         }
    //     }
    //
    //     return await query.FirstOrDefaultAsync(filter, cancellationToken);
    // }
    //
    // public async Task<IReadOnlyList<User>> PaginatedListAllAsync(
    //     int offset,
    //     int limit,
    //     CancellationToken cancellationToken = default,
    //     params Expression<Func<User, object>>[]? includesProperties)
    // {
    //     var query = _context.Users.AsQueryable().AsNoTracking();
    //
    //     if (includesProperties != null)
    //     {
    //         foreach (var includeProperty in includesProperties)
    //         {
    //             query = query.Include(includeProperty);
    //         }
    //     }
    //
    //     return await query
    //         .OrderBy(x => x.RegisteredAt)
    //         .Skip(offset)
    //         .Take(limit)
    //         .ToListAsync(cancellationToken);
    // }
    //
    // public async Task<IReadOnlyList<User>> PaginatedListAsync(
    //     Expression<Func<User, bool>>? filter,
    //     int offset,
    //     int limit,
    //     CancellationToken cancellationToken = default,
    //     params Expression<Func<User, object>>[]? includesProperties)
    // {
    //     var query = _context.Users.AsQueryable().AsNoTracking();
    //
    //     if (filter != null)
    //     {
    //         query = query.Where(filter);
    //     }
    //
    //     if (includesProperties != null)
    //     {
    //         foreach (var includeProperty in includesProperties)
    //         {
    //             query = query.Include(includeProperty);
    //         }
    //     }
    //
    //     return await query
    //         .OrderBy(x => x.RegisteredAt)
    //         .Skip(offset)
    //         .Take(limit)
    //         .ToListAsync(cancellationToken);
    // }
    //
    // public Task UpdateAsync(User entity, CancellationToken cancellationToken = default)
    // {
    //     _context.Users.Update(entity);
    //     return Task.CompletedTask;
    // }
    //
    // public Task DeleteAsync(User entity, CancellationToken cancellationToken = default)
    // {
    //     _context.Users.Remove(entity);
    //     return Task.CompletedTask;
    // }
    //
    // public async Task<int> CountAsync(Expression<Func<User, bool>>? filter, CancellationToken cancellationToken = default)
    // {
    //     var query = _context.Users.AsQueryable();
    //
    //     if (filter != null)
    //     {
    //         query = query.Where(filter);
    //     }
    //
    //     return await query.CountAsync(cancellationToken);
    // }


    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default, bool includeProfile = false)
    {
        try
        {
            var user = await _context.Database
                .SqlQuery<User>($"""
                                 SELECT * FROM "Users" WHERE "Id" = {id.ToString()}
                                 """)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            if (!includeProfile || user is null)
            {
                return user;
            }

            var role = await _context.Roles
                .FromSql($"""
                          SELECT * FROM "Roles" WHERE "Id" = {user.RoleId.ToString()}
                          """)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            if (role?.Name == AppRoles.FreelancerRole)
            {
                user.FreelancerProfile = await _context.FreelancerProfiles
                    .FromSql($"""
                              SELECT * FROM "FreelancerProfiles" WHERE "UserId" = {user.Id.ToString()}
                              """)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(cancellationToken);
            }

            if (role?.Name == AppRoles.FreelancerRole)
            {
                user.EmployerProfile = await _context.EmployerProfiles
                    .FromSql($"""
                              SELECT * FROM "FreelancerProfiles" WHERE "UserId" = {user.Id.ToString()}
                              """)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(cancellationToken);
            }

            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to get user by id. Error: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to get user by id. Error: {ex.Message}");
        }
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FromSql($"""
                      SELECT * FROM "Users" WHERE "Email" = {email}
                      """)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);
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
                var refreshTokenExpiryTimeString = $"'{refreshTokenExpiryTime:yyyy-MM-dd HH:mm:ss.ffffff}Z'";

                rowsAffected = await _context.Database.ExecuteSqlAsync(
                    $"""
                     UPDATE "Users"
                     SET "RefreshToken" = {refreshToken},
                         "RefreshTokenExpiryTime" = {refreshTokenExpiryTimeString}
                     WHERE "Id" = {id.ToString()}
                     """,
                    cancellationToken);
            }
            else
            {
                rowsAffected = await _context.Database.ExecuteSqlAsync(
                    $"""
                     UPDATE "Users"
                     SET "RefreshToken" = NULL,
                         "RefreshTokenExpiryTime" = NULL
                     WHERE "Id" = {id.ToString()}
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

    public async Task UpdateIsEmailConfirmedAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var rowsAffected = await _context.Database.ExecuteSqlAsync(
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
            var rowsAffected = await _context.Database.ExecuteSqlAsync(
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

    public async Task CreateAsync(User user, CancellationToken cancellationToken = default)
    {
        try
        {
            var rowsAffected = await _context.Database.ExecuteSqlAsync(
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
            var rowsAffected = await _context.Database.ExecuteSqlAsync(
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