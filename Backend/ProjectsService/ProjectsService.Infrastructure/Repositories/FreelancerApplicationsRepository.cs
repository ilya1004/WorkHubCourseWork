using System.Runtime.CompilerServices;
using Elastic.Clients.Elasticsearch.MachineLearning;
using ProjectsService.Domain.Enums;
using ProjectsService.Infrastructure.Data;

namespace ProjectsService.Infrastructure.Repositories;

public class FreelancerApplicationsRepository : IFreelancerApplicationsRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<FreelancerApplicationsRepository> _logger;

    public FreelancerApplicationsRepository(
        ApplicationDbContext context,
        ILogger<FreelancerApplicationsRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<FreelancerApplication?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default,
        bool includeRelatedEntities = false)
    {
        try
        {
            var freelancerApplication = await _context.FreelancerApplications
                .FromSqlInterpolated($"""
                          SELECT * FROM "FreelancerApplications" WHERE "Id" = {id.ToString()}
                          """)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            if (freelancerApplication is null || !includeRelatedEntities)
            {
                return freelancerApplication;
            }

            freelancerApplication.Project = await _context.Projects
                .FromSqlInterpolated($"""
                          SELECT * FROM "Projects" WHERE "Id" = {id.ToString()}
                          """)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            return freelancerApplication;
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to get freelancer application by id. Error: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to get freelancer application by id. Error: {ex.Message}");
        }
    }

    public async Task<IReadOnlyList<FreelancerApplication>> GetByProjectIdAsync(
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.FreelancerApplications
                .FromSqlInterpolated($"""
                          SELECT * FROM "FreelancerApplications" 
                          WHERE "ProjectId" = {projectId.ToString()}
                          ORDER BY "Id" DESC
                          """)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to get applications by project id. Error: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to get applications by project id. Error: {ex.Message}");
        }
    }

    public async Task<IReadOnlyList<FreelancerApplication>> GetByFreelancerUserIdAsync(
        Guid freelancerUserId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.FreelancerApplications
                .FromSqlInterpolated($"""
                          SELECT * FROM "FreelancerApplications" 
                          WHERE "FreelancerUserId" = {freelancerUserId.ToString()}
                          ORDER BY "Id" DESC
                          """)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to get applications by freelancer user id. Error: {Message}", ex.Message);
            throw new InvalidOperationException(
                $"Failed to get applications by freelancer user id. Error: {ex.Message}");
        }
    }

    public async Task<IReadOnlyList<FreelancerApplication>> GetAllPaginatedAsync(
        int offset,
        int limit,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.FreelancerApplications
                .FromSqlInterpolated($"""
                          SELECT * FROM "FreelancerApplications"
                          ORDER BY "Id"
                          LIMIT {limit} OFFSET {offset}
                          """)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to get freelancer applications. Error: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to get freelancer applications. Error: {ex.Message}");
        }
    }

    public async Task<IReadOnlyList<FreelancerApplication>> GetAllPaginatedByProjectAsync(
        Guid projectId,
        int offset,
        int limit,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.FreelancerApplications
                .FromSqlInterpolated($"""
                          SELECT * FROM "FreelancerApplications"
                          ORDER BY "Id"
                          WHERE "ProjectId" = {projectId.ToString()}
                          LIMIT {limit} OFFSET {offset}
                          """)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to get freelancer applications. Error: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to get freelancer applications. Error: {ex.Message}");
        }
    }

    public async Task<IReadOnlyList<FreelancerApplication>> GetByFilterAsync(
        DateTime? startDate,
        DateTime? endDate,
        ApplicationStatus? applicationStatus,
        int limit,
        int offset,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var sql = """
                      SELECT * FROM "FreelancerApplications"
                      WHERE 1 = 1
                      """;

            var conditions = new List<string>();

            if (startDate.HasValue)
            {
                conditions.Add($""" "CreatedAt" >= {startDate.Value} """);
            }

            if (endDate.HasValue)
            {
                var endOfDay = endDate.Value.AddDays(1).AddTicks(-1);
                conditions.Add($""" "CreatedAt" <= {endOfDay} """);
            }

            if (applicationStatus.HasValue)
            {
                conditions.Add($""" "Status" = {applicationStatus.Value.ToString()} """);
            }

            if (conditions.Any())
            {
                sql += string.Join(" AND ", conditions);
            }

            sql += $"""
                    ORDER BY "Id" DESC
                    LIMIT {limit} OFFSET {offset}
                    """;

            return await _context.FreelancerApplications
                .FromSqlInterpolated(FormattableStringFactory.Create(sql))
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to get freelancer applications by filter. StartDate: {StartDate}, EndDate: {EndDate}, Status: {Status}",
                startDate, endDate, applicationStatus);
            throw new InvalidOperationException("Failed to retrieve freelancer applications by filter.", ex);
        }
    }

    public async Task<int> CountByFilterAsync(
        DateTime? startDate,
        DateTime? endDate,
        ApplicationStatus? applicationStatus,
        int limit,
        int offset,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var sql = """
                      SELECT COUNT(*) AS "Value" FROM "FreelancerApplications"
                      WHERE 1 = 1
                      """;

            var conditions = new List<string>();

            if (startDate.HasValue)
            {
                conditions.Add($""" "CreatedAt" >= {startDate.Value} """);
            }

            if (endDate.HasValue)
            {
                var endOfDay = endDate.Value.AddDays(1).AddTicks(-1);
                conditions.Add($""" "CreatedAt" <= {endOfDay} """);
            }

            if (applicationStatus.HasValue)
            {
                conditions.Add($""" "Status" = {applicationStatus.Value.ToString()} """);
            }

            if (conditions.Any())
            {
                sql += string.Join(" AND ", conditions);
            }

            return await _context.Database
                .SqlQueryRaw<int>(sql)
                .SingleAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to count freelancer applications by filter.");
            throw new InvalidOperationException("Failed to count freelancer applications by filter.", ex);
        }
    }

    public async Task<int> CountAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Database
                .SqlQuery<int>(
                    $"""
                        SELECT COUNT(*) AS "Value" FROM "FreelancerApplications"
                     """)
                .SingleAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to get freelancer applications count. Error: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to get freelancer applications count. Error: {ex.Message}");
        }
    }

    public async Task<int> CountByProjectAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Database
                .SqlQuery<int>(
                    $"""
                        SELECT COUNT(*) AS "Value" FROM "FreelancerApplications"
                        WHERE "ProjectId" = {projectId.ToString()}
                     """)
                .SingleAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to get freelancer applications count. Error: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to get freelancer applications count. Error: {ex.Message}");
        }
    }

    public async Task<bool> AnyByProjectIdAndApplicationStatus(
        Guid projectId,
        ApplicationStatus status,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Database
                .SqlQuery<bool>($"""
                                 SELECT EXISTS (
                                     SELECT 1
                                     FROM "FreelancerApplications"
                                     WHERE "Status" = {status.ToString()} AND "ProjectId" = {projectId.ToString()}
                                 )
                                 """)
                .FirstOrDefaultAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to get any freelancer applications by status. Error: {Message}", ex.Message);
            throw new InvalidOperationException(
                $"Failed to get any freelancer applications by status. Error: {ex.Message}");
        }
    }

    public async Task UpdateRejectedStatusWhenNotAcceptedAsync(
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.Database.ExecuteSqlInterpolatedAsync(
                $"""
                 UPDATE "FreelancerApplications"
                 SET "Status" = {nameof(ApplicationStatus.Rejected)},
                 WHERE "ProjectId" = {projectId.ToString()} AND "Status" != "Accepted"
                 """,
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to update freelancer applications status. Error: {Message}", ex.Message);
            throw new InvalidOperationException(
                $"Failed to update freelancer applications status. Error: {ex.Message}");
        }
    }

    public async Task CreateAsync(FreelancerApplication application, CancellationToken cancellationToken = default)
    {
        try
        {
            var rowsAffected = await _context.Database.ExecuteSqlInterpolatedAsync(
                $"""
                 INSERT INTO "FreelancerApplications" (
                     "Id", "CreatedAt", "Status", "CvId", "FreelancerUserId", "ProjectId"
                 )
                 VALUES (
                     {application.Id},
                     {application.CreatedAt},
                     {application.Status.ToString()},
                     {application.CvId},
                     {application.FreelancerUserId},
                     {application.ProjectId}
                 )
                 """,
                cancellationToken);

            if (rowsAffected != 1)
            {
                _logger.LogError("Failed to create freelancer application. Affected [{rowsAffected}] rows",
                    rowsAffected);
                throw new InvalidOperationException(
                    $"Failed to create freelancer application. Affected [{rowsAffected}] rows");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to create freelancer application. Error: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to create freelancer application. Error: {ex.Message}");
        }
    }

    public async Task UpdateAsync(FreelancerApplication application, CancellationToken cancellationToken = default)
    {
        try
        {
            var rowsAffected = await _context.Database.ExecuteSqlInterpolatedAsync(
                $"""
                 UPDATE "FreelancerApplications"
                 SET 
                     "Status" = {application.Status.ToString()}, 
                     "CvId" = {application.CvId}
                 WHERE "Id" = {application.Id.ToString()}
                 """,
                cancellationToken);

            if (rowsAffected != 1)
            {
                _logger.LogError("Failed to update freelancer application. Affected [{rowsAffected}] rows",
                    rowsAffected);
                throw new InvalidOperationException(
                    $"Failed to update freelancer application. Affected [{rowsAffected}] rows");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to update freelancer application. Error: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to update freelancer application. Error: {ex.Message}");
        }
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var rowsAffected = await _context.Database.ExecuteSqlInterpolatedAsync(
                $"""
                 DELETE FROM "FreelancerApplications"
                 WHERE "Id" = {id.ToString()}
                 """,
                cancellationToken);

            if (rowsAffected != 1)
            {
                _logger.LogError("Failed to delete freelancer application. Affected [{rowsAffected}] rows",
                    rowsAffected);
                throw new InvalidOperationException(
                    $"Failed to delete freelancer application. Affected [{rowsAffected}] rows");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to delete freelancer application. Error: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to delete freelancer application. Error: {ex.Message}");
        }
    }
}