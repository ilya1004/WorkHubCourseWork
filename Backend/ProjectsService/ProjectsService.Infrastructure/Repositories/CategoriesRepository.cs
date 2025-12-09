using ProjectsService.Infrastructure.Data;

namespace ProjectsService.Infrastructure.Repositories;

public class CategoriesRepository : ICategoriesRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CategoriesRepository> _logger;

    public CategoriesRepository(
        ApplicationDbContext context,
        ILogger<CategoriesRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Categories
                .FromSqlInterpolated($"""
                          SELECT * FROM "Categories" WHERE "Id" = {id.ToString()}
                          """)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to get category by id. Error: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to get category by id. Error: {ex.Message}");
        }
    }

    public async Task<Category?> GetByName(string name, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Categories
                .FromSqlInterpolated($"""
                          SELECT * FROM "Categories" WHERE "Name" = {name}
                          """)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to get category by id. Error: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to get category by id. Error: {ex.Message}");
        }
    }

    public async Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Categories
                .FromSqlInterpolated($"""
                          SELECT * FROM "Categories"
                          ORDER BY "Id"
                          """)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to get categories. Error: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to get categories. Error: {ex.Message}");
        }
    }

    public async Task<IReadOnlyList<Category>> GetAllPaginatedAsync(int offset, int limit, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Categories
                .FromSqlInterpolated($"""
                          SELECT * FROM "Categories"
                          ORDER BY "Id"
                          LIMIT {limit} OFFSET {offset}
                          """)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to get categories. Error: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to get categories. Error: {ex.Message}");
        }
    }

    public async Task<int> CountAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Database
                .SqlQuery<int>(
                    $"""
                        SELECT COUNT(*) FROM "Categories"
                     """)
                .FirstOrDefaultAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to get categories count. Error: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to get categories count. Error: {ex.Message}");
        }
    }

    public async Task CreateAsync(Category category, CancellationToken cancellationToken = default)
    {
        try
        {
            var rowsAffected = await _context.Database.ExecuteSqlInterpolatedAsync(
                $"""
                 INSERT INTO "Categories" ("Id", "Name")
                 VALUES ({category.Id}, {category.Name})
                 """,
                cancellationToken);

            if (rowsAffected != 1)
            {
                _logger.LogError("Failed to create category. Affected [{rowsAffected}] rows", rowsAffected);
                throw new InvalidOperationException($"Failed to create category. Affected [{rowsAffected}] rows");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to create category. Error: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to create category. Error: {ex.Message}");
        }
    }

    public async Task UpdateAsync(Category category, CancellationToken cancellationToken = default)
    {
        try
        {
            var rowsAffected = await _context.Database.ExecuteSqlInterpolatedAsync(
                $"""
                 UPDATE "Categories"
                 SET "Name" = {category.Name}
                 WHERE "Id" = {category.ToString()}
                 """,
                cancellationToken);

            if (rowsAffected != 1)
            {
                _logger.LogError("Failed to update category. Affected [{rowsAffected}] rows", rowsAffected);
                throw new InvalidOperationException($"Failed to update category. Affected [{rowsAffected}] rows");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to update category. Error: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to update category. Error: {ex.Message}");
        }
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var rowsAffected = await _context.Database.ExecuteSqlInterpolatedAsync(
                $"""
                 DELETE FROM "Categories"
                 WHERE "Id" = {id.ToString()}
                 """,
                cancellationToken);

            if (rowsAffected != 1)
            {
                _logger.LogError("Failed to delete category. Affected [{rowsAffected}] rows", rowsAffected);
                throw new InvalidOperationException($"Failed to delete category. Affected [{rowsAffected}] rows");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to delete category. Error: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to delete category. Error: {ex.Message}");
        }
    }
}