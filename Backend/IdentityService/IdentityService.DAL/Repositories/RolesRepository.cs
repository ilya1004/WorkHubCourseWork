namespace IdentityService.DAL.Repositories;

public class RolesRepository : IRolesRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<RolesRepository> _logger;

    public RolesRepository(ApplicationDbContext context, ILogger<RolesRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Role?> GetByNameAsync(string roleName, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Roles
                .FromSql($"""
                          SELECT * FROM "Roles" WHERE "Name" = {roleName}
                          """)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to get role by name. Error: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to get role by name. Error: {ex.Message}");
        }
    }

    public async Task CreateAsync(Role role, CancellationToken cancellationToken = default)
    {
        try
        {
            var rowsAffected = await _context.Database.ExecuteSqlAsync(
                $"""
                 INSERT INTO "Roles" ("Id", "Name")
                 VALUES ({role.Id}, {role.Name})
                 """,
                cancellationToken);

            if (rowsAffected != 1)
            {
                _logger.LogError("Failed to create role. Affected [{rowsAffected}] rows", rowsAffected);
                throw new InvalidOperationException($"Failed to create role. Affected [{rowsAffected}] rows");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to create role. Error: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to create role. Error: {ex.Message}");
        }
    }
}