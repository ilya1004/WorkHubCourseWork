using IdentityService.DAL.Abstractions.Repositories;
using IdentityService.DAL.Data;
using Microsoft.Extensions.Logging;

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
                .FromSql($"""
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
                .FromSql($"""
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

    public async Task CreateAsync(EmployerIndustry employerIndustry, CancellationToken cancellationToken = default)
    {
        try
        {
            var rowsAffected = await _context.Database.ExecuteSqlAsync(
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
}