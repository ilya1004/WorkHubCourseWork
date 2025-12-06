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

    public async Task<FreelancerApplication?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.FreelancerApplications
                .FromSql($"""
                          SELECT * FROM "FreelancerApplications" WHERE "Id" = {id.ToString()}
                          """)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);
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
                .FromSql($"""
                          SELECT * FROM "FreelancerApplications" 
                          WHERE "ProjectId" = {projectId.ToString()}
                          ORDER BY "CreatedAt" DESC
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
                .FromSql($"""
                          SELECT * FROM "FreelancerApplications" 
                          WHERE "FreelancerUserId" = {freelancerUserId.ToString()}
                          ORDER BY "CreatedAt" DESC
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
            await _context.Database.ExecuteSqlAsync(
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
            var rowsAffected = await _context.Database.ExecuteSqlAsync(
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
            var rowsAffected = await _context.Database.ExecuteSqlAsync(
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
            var rowsAffected = await _context.Database.ExecuteSqlAsync(
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