using ProjectsService.Domain.Enums;
using ProjectsService.Infrastructure.Data;

namespace ProjectsService.Infrastructure.Repositories;

public class LifecyclesRepository : ILifecyclesRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<LifecyclesRepository> _logger;

    public LifecyclesRepository(ApplicationDbContext context, ILogger<LifecyclesRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Lifecycle?> GetByProjectIdAsync(
        Guid projectId,
        CancellationToken cancellationToken = default,
        bool includeProject = false)
    {
        try
        {
            var lifecycle = await _context.Lifecycles
                .FromSqlInterpolated($"""
                          SELECT * FROM "Lifecycles" WHERE "ProjectId" = {projectId.ToString()}
                          """)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            if (lifecycle is not null && includeProject)
            {
                lifecycle.Project = await _context.Projects
                    .FromSqlInterpolated($"""
                              SELECT * FROM "Projects" WHERE "Id" = {projectId.ToString()}
                              """)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(cancellationToken);
            }

            return lifecycle;
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to get lifecycle by project id. Error: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to get lifecycle by project id. Error: {ex.Message}");
        }
    }

    public async Task UpdateStatusByProjectIdAsync(
        Guid projectId,
        ProjectStatus status,
        DateTime updatedAt,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var rowsAffected = await _context.Database.ExecuteSqlInterpolatedAsync(
                $"""
                 UPDATE "Lifecycles"
                 SET 
                     "UpdatedAt" = {updatedAt},
                     "ProjectStatus" = {status.ToString()}
                 WHERE "Id" = {projectId.ToString()}
                 """,
                cancellationToken);

            if (rowsAffected != 1)
            {
                _logger.LogError("Failed to update lifecycle project status. Affected [{rowsAffected}] rows", rowsAffected);
                throw new InvalidOperationException($"Failed to update lifecycle project status. Affected [{rowsAffected}] rows");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to update lifecycle project status. Error: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to update lifecycle project status. Error: {ex.Message}");
        }
    }

    public async Task CreateAsync(Lifecycle lifecycle, CancellationToken cancellationToken = default)
    {
        try
        {
            var rowsAffected = await _context.Database.ExecuteSqlInterpolatedAsync(
                $"""
                 INSERT INTO "Lifecycles" (
                     "Id", "CreatedAt", "ApplicationsStartDate", "ApplicationsDeadline",
                     "WorkStartDate", "WorkDeadline", "AcceptanceStatus", "ProjectStatus", "ProjectId"
                 )
                 VALUES (
                     {lifecycle.Id},
                     {lifecycle.CreatedAt},
                     {lifecycle.ApplicationsStartDate},
                     {lifecycle.ApplicationsDeadline},
                     {lifecycle.WorkStartDate},
                     {lifecycle.WorkDeadline},
                     {lifecycle.AcceptanceStatus.ToString()},
                     {lifecycle.ProjectStatus.ToString()},
                     {lifecycle.ProjectId}
                 )
                 """,
                cancellationToken);

            if (rowsAffected != 1)
            {
                _logger.LogError("Failed to create lifecycle. Affected [{rowsAffected}] rows", rowsAffected);
                throw new InvalidOperationException($"Failed to create lifecycle. Affected [{rowsAffected}] rows");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to create lifecycle. Error: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to create lifecycle. Error: {ex.Message}");
        }
    }

    public async Task UpdateAsync(Lifecycle lifecycle, CancellationToken cancellationToken = default)
    {
        try
        {
            var rowsAffected = await _context.Database.ExecuteSqlInterpolatedAsync(
                $"""
                 UPDATE "Lifecycles"
                 SET 
                     "UpdatedAt" = {lifecycle.UpdatedAt},
                     "ApplicationsStartDate" = {lifecycle.ApplicationsStartDate},
                     "ApplicationsDeadline" = {lifecycle.ApplicationsDeadline},
                     "WorkStartDate" = {lifecycle.WorkStartDate},
                     "WorkDeadline" = {lifecycle.WorkDeadline},
                     "AcceptanceStatus" = {lifecycle.AcceptanceStatus.ToString()},
                     "ProjectStatus" = {lifecycle.ProjectStatus.ToString()}
                 WHERE "Id" = {lifecycle.Id.ToString()}
                 """,
                cancellationToken);

            if (rowsAffected != 1)
            {
                _logger.LogError("Failed to update lifecycle. Affected [{rowsAffected}] rows", rowsAffected);
                throw new InvalidOperationException($"Failed to update lifecycle. Affected [{rowsAffected}] rows");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to update lifecycle. Error: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to update lifecycle. Error: {ex.Message}");
        }
    }

    public async Task UpdateProjectStatusAsync(
        Guid projectId,
        ProjectStatus projectStatus,
        DateTime updatedAt,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var rowsAffected = await _context.Database.ExecuteSqlInterpolatedAsync(
                $"""
                 UPDATE "Lifecycles"
                 SET 
                     "ProjectStatus" = {projectStatus.ToString()}, 
                     "UpdatedAt" = {updatedAt}
                 WHERE "ProjectId" = {projectId.ToString()}
                 """,
                cancellationToken);

            if (rowsAffected != 1)
            {
                _logger.LogError("Failed to update project status. Affected [{rowsAffected}] rows", rowsAffected);
                throw new InvalidOperationException(
                    $"Failed to update project status. Affected [{rowsAffected}] rows");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to update project status. Error: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to update project status. Error: {ex.Message}");
        }
    }

    public async Task UpdateAcceptanceStatusAsync(
        Guid projectId,
        ProjectAcceptanceStatus projectAcceptanceStatus,
        DateTime updatedAt,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var rowsAffected = await _context.Database.ExecuteSqlInterpolatedAsync(
                $"""
                 UPDATE "Lifecycles"
                 SET 
                     "AcceptanceStatus" = {projectAcceptanceStatus.ToString()}, 
                     "UpdatedAt" = {updatedAt}
                 WHERE "ProjectId" = {projectId.ToString()}
                 """,
                cancellationToken);

            if (rowsAffected != 1)
            {
                _logger.LogError("Failed to update project acceptance status. Affected [{rowsAffected}] rows",
                    rowsAffected);
                throw new InvalidOperationException(
                    $"Failed to update project acceptance status. Affected [{rowsAffected}] rows");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to update acceptance project status. Error: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to update acceptance project status. Error: {ex.Message}");
        }
    }
}