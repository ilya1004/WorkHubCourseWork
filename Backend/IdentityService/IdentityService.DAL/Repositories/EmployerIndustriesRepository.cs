namespace IdentityService.DAL.Repositories;

public class EmployerIndustriesRepository : IEmployerIndustriesRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<EmployerIndustriesRepository> _logger;

    public EmployerIndustriesRepository(ApplicationDbContext context, ILogger<EmployerIndustriesRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<EmployerIndustry?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.EmployerIndustries
                .FromSqlInterpolated($"""
                          SELECT * FROM "EmployerIndustries" WHERE "Id" = {id.ToString()}
                          """)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to get employer industry by id. Error: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to get employer industry by id. Error: {ex.Message}");
        }
    }

    public async Task<EmployerIndustry?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.EmployerIndustries
                .FromSqlInterpolated($"""
                          SELECT * FROM "EmployerIndustries" WHERE "Name" = {name}
                          """)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to get employer industry by name. Error: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to get employer industry by name. Error: {ex.Message}");
        }
    }

    public async Task<IReadOnlyList<EmployerIndustry>> GetAllPaginatedAsync(
        int offset,
        int limit,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.EmployerIndustries
                .FromSqlInterpolated($"""
                          SELECT * FROM "EmployerIndustries"
                          ORDER BY "Id"
                          LIMIT {limit} OFFSET {offset}
                          """)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to get paginated employer industries. Error: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to get paginated employer industries. Error: {ex.Message}");
        }
    }

    public async Task<int> CountAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Database
                .SqlQuery<int>($"""
                                SELECT COUNT(*) AS "Value" FROM "EmployerIndustries"
                                """)
                .SingleAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to get employer industries count. Error: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to get employer industries count. Error: {ex.Message}");
        }
    }

    public async Task CreateAsync(EmployerIndustry employerIndustry, CancellationToken cancellationToken = default)
    {
        try
        {
            var rowsAffected = await _context.Database.ExecuteSqlInterpolatedAsync(
                $"""
                 INSERT INTO "EmployerIndustries" ("Id", "Name")
                 VALUES ({employerIndustry.Id}, {employerIndustry.Name})
                 """,
                cancellationToken);

            if (rowsAffected != 1)
            {
                _logger.LogError("Failed to create employer industry. Affected [{rowsAffected}] rows", rowsAffected);
                throw new InvalidOperationException($"Failed to create employer industry. Affected [{rowsAffected}] rows");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to create employer industry. Error: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to create employer industry. Error: {ex.Message}");
        }
    }

    public async Task UpdateAsync(EmployerIndustry employerIndustry, CancellationToken cancellationToken = default)
    {
        try
        {
            var rowsAffected = await _context.Database.ExecuteSqlInterpolatedAsync(
                $"""
                 UPDATE "EmployerIndustries"
                 SET "Name" = {employerIndustry.Name}
                 WHERE "Id" = {employerIndustry.ToString()}
                 """,
                cancellationToken);

            if (rowsAffected != 1)
            {
                _logger.LogError("Failed to update employer industry. Affected [{rowsAffected}] rows", rowsAffected);
                throw new InvalidOperationException($"Failed to update employer industry. Affected [{rowsAffected}] rows");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to update employer industry. Error: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to update employer industry. Error: {ex.Message}");
        }
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var rowsAffected = await _context.Database.ExecuteSqlInterpolatedAsync(
                $"""
                 DELETE FROM "EmployerIndustries"
                 WHERE "Id" = {id.ToString()}
                 """,
                cancellationToken);

            if (rowsAffected != 1)
            {
                _logger.LogError("Failed to delete employer industry. Affected [{rowsAffected}] rows", rowsAffected);
                throw new InvalidOperationException($"Failed to delete employer industry. Affected [{rowsAffected}] rows");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to delete employer industry. Error: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to delete employer industry. Error: {ex.Message}");
        }
    }
}