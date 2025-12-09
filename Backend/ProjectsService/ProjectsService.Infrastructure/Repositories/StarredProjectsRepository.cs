using ProjectsService.Infrastructure.Data;

namespace ProjectsService.Infrastructure.Repositories;

public class StarredProjectsRepository : IStarredProjectsRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<StarredProjectsRepository> _logger;

    public StarredProjectsRepository(
        ApplicationDbContext context,
        ILogger<StarredProjectsRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> IsStarredAsync(
        Guid projectId,
        Guid freelancerUserId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.StarredProjects
                .FromSqlInterpolated($"""
                          SELECT 1 FROM "StarredProjects" 
                          WHERE "ProjectId" = {projectId.ToString()} 
                            AND "FreelancerUserId" = {freelancerUserId.ToString()}
                          """)
                .AnyAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to check if project is starred. Error: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to check if project is starred. Error: {ex.Message}");
        }
    }

    public async Task<IReadOnlyList<StarredProject>> GetByFreelancerUserIdAsync(
        Guid freelancerUserId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.StarredProjects
                .FromSqlInterpolated($"""
                          SELECT * FROM "StarredProjects" 
                          WHERE "FreelancerUserId" = {freelancerUserId.ToString()}
                          ORDER BY "Id" DESC
                          """)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to get starred projects by freelancer user id. Error: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to get starred projects. Error: {ex.Message}");
        }
    }

    public async Task CreateAsync(StarredProject starredProject, CancellationToken cancellationToken = default)
    {
        try
        {
            var rowsAffected = await _context.Database.ExecuteSqlInterpolatedAsync(
                $"""
                 INSERT INTO "StarredProjects" ("Id", "ProjectId", "FreelancerUserId")
                 VALUES ({starredProject.Id}, {starredProject.ProjectId}, {starredProject.FreelancerUserId})
                 """,
                cancellationToken);

            if (rowsAffected != 1)
            {
                _logger.LogError("Failed to star project. Affected [{rowsAffected}] rows", rowsAffected);
                throw new InvalidOperationException($"Failed to star project. Affected [{rowsAffected}] rows");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to star project. Error: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to star project. Error: {ex.Message}");
        }
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var rowsAffected = await _context.Database.ExecuteSqlInterpolatedAsync(
                $"""
                 DELETE FROM "StarredProjects"
                 WHERE "Id" = {id.ToString()}
                 """,
                cancellationToken);

            if (rowsAffected != 1)
            {
                _logger.LogError("Failed to delete starred project by id. Affected [{rowsAffected}] rows",
                    rowsAffected);
                throw new InvalidOperationException(
                    $"Failed to delete starred project. Affected [{rowsAffected}] rows");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to delete starred project by id. Error: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to delete starred project. Error: {ex.Message}");
        }
    }
}