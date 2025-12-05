using ProjectsService.Domain.Enums;
using ProjectsService.Infrastructure.Data;

namespace ProjectsService.Infrastructure.Repositories;

public class ReportsRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ReportsRepository> _logger;

    public ReportsRepository(
        ApplicationDbContext context,
        ILogger<ReportsRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Report?> GetByUserId(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Reports
                .FromSql($"""
                          SELECT * FROM "Reports" WHERE "Id" = {id.ToString()}
                          """)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to get report by id. Error: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to get report by id. Error: {ex.Message}");
        }
    }

    public async Task<IReadOnlyList<Report>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Reports
                .FromSql($"""
                          SELECT * FROM "Reports"
                          ORDER BY "Id"
                          """)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to get reports. Error: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to get reports. Error: {ex.Message}");
        }
    }

    public async Task CreateAsync(Report report, CancellationToken cancellationToken = default)
    {
        try
        {
            var rowsAffected = await _context.Database.ExecuteSqlAsync(
                $"""
                 INSERT INTO "Reports" ("Id", "Description", "Status", "ProjectId", "ReporterUserId")
                 VALUES ({report.Id}, {report.Description}, {report.Status}, {report.ProjectId}, {report.ReporterUserId})
                 """,
                cancellationToken);

            if (rowsAffected != 1)
            {
                _logger.LogError("Failed to create report. Affected [{rowsAffected}] rows", rowsAffected);
                throw new InvalidOperationException($"Failed to create report. Affected [{rowsAffected}] rows");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to create report. Error: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to create report. Error: {ex.Message}");
        }
    }

    public async Task UpdateStatusAsync(Guid reportId, ReportStatus reportStatus, CancellationToken cancellationToken = default)
    {
        try
        {
            var rowsAffected = await _context.Database.ExecuteSqlAsync(
                $"""
                 UPDATE "Reports"
                 SET "Status" = {reportStatus}
                 WHERE "Id" = {reportId.ToString()}
                 """,
                cancellationToken);

            if (rowsAffected != 1)
            {
                _logger.LogError("Failed to update report status. Affected [{rowsAffected}] rows", rowsAffected);
                throw new InvalidOperationException($"Failed to update report status. Affected [{rowsAffected}] rows");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to update report status. Error: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to update report status. Error: {ex.Message}");
        }
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var rowsAffected = await _context.Database.ExecuteSqlAsync(
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