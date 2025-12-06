using ProjectsService.Domain.Enums;
using ProjectsService.Domain.Models;
using ProjectsService.Infrastructure.Data;

namespace ProjectsService.Infrastructure.Repositories;

public class ProjectsRepository : IProjectsRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ProjectsRepository> _logger;

    public ProjectsRepository(
        ApplicationDbContext context,
        ILogger<ProjectsRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Project?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default,
        bool includeRelatedEntities = false)
    {
        try
        {
            var project = await _context.Projects
                .FromSql($"""
                          SELECT * FROM "Projects" WHERE "Id" = {id.ToString()}
                          """)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            if (project is null || !includeRelatedEntities)
                return project;

            project.Lifecycle = await _context.Lifecycles
                .FromSql($"""
                          SELECT * FROM "Lifecycles" WHERE "ProjectId" = {id.ToString()}
                          """)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            if (project.CategoryId.HasValue)
            {
                project.Category = await _context.Categories
                    .FromSql($"""
                              SELECT * FROM "Categories" WHERE "Id" = {project.CategoryId.Value.ToString()}
                              """)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(cancellationToken);
            }

            return project;
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to get project by id. Error: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to get project by id. Error: {ex.Message}");
        }
    }

    public async Task<IReadOnlyList<Project>> GetByEmployerUserIdAsync(
        Guid employerUserId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Projects
                .FromSql($"""
                          SELECT * FROM "Projects" 
                          WHERE "EmployerUserId" = {employerUserId.ToString()}
                          ORDER BY "Id" DESC
                          """)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to get projects by employer id. Error: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to get projects by employer id. Error: {ex.Message}");
        }
    }

    public async Task<Project?> GetByEmployerAndTitleAsync(
        Guid employerUserId,
        string title,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Projects
                .FromSql($"""
                          SELECT * FROM "Projects" 
                          WHERE "EmployerUserId" = {employerUserId.ToString()} AND "Title" = {title}
                          """)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to get projects by employer id. Error: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to get projects by employer id. Error: {ex.Message}");
        }
    }

    public async Task<IReadOnlyList<ProjectModel>> GetByIsActiveAsync(
        bool? isActive,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Database
                .SqlQuery<ProjectModel>($"""
                                         SELECT * FROM "ProjectInfo"
                                         WHERE "IsActive" = {isActive}
                                         ORDER BY "Id" DESC
                                         """)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to get projects by is active status. Error: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to get projects by is active status. Error: {ex.Message}");
        }
    }

    public async Task<IReadOnlyList<ProjectModel>> GetFilteredAsync(
        Guid? categoryId = null,
        Guid? employerUserId = null,
        Guid? freelancerUserId = null,
        ProjectStatus? projectStatus = null,
        ProjectAcceptanceStatus? acceptanceStatus = null,
        string? searchTitle = null,
        bool? isActive = null,
        int offset = 0,
        int limit = 20,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var sql = """
                      SELECT * FROM "ProjectModel"
                      WHERE 1 = 1
                      """;

            var conditions = new List<string>();

            if (categoryId.HasValue)
            {
                conditions.Add($""" "CategoryId" = {categoryId.Value.ToString()} """);
            }

            if (employerUserId.HasValue)
            {
                conditions.Add($""" "EmployerUserId" = {employerUserId.Value.ToString()} """);
            }

            if (freelancerUserId.HasValue)
            {
                conditions.Add($""" "FreelancerUserId" = {freelancerUserId.Value.ToString()} """);
            }

            if (projectStatus.HasValue)
            {
                conditions.Add($""" "ProjectStatus" = {projectStatus.Value.ToString()} """);
            }

            if (acceptanceStatus.HasValue)
            {
                conditions.Add($""" "AcceptanceStatus" = {acceptanceStatus.Value.ToString()} """);
            }

            if (!string.IsNullOrWhiteSpace(searchTitle))
            {
                var search = searchTitle.Trim().ToLower();
                conditions.Add($"""
                                LOWER("Title") LIKE '%{search}%'
                                """);
            }

            if (isActive.HasValue)
            {
                conditions.Add($""" "IsActive" = {isActive.Value.ToString()} """);
            }

            if (conditions.Count != 0)
            {
                sql += string.Join(" AND ", conditions);
            }

            sql += $"""
                    ORDER BY "CreatedAt" DESC
                    LIMIT {limit} OFFSET {offset}
                    """;

            var result = await _context.Set<ProjectModel>()
                .FromSqlRaw(sql)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to get filtered projects. Error: {Message}", ex.Message);
            throw new InvalidOperationException("Failed to retrieve projects with filters.", ex);
        }
    }

    public async Task CreateAsync(Project project, CancellationToken cancellationToken = default)
    {
        try
        {
            var rowsAffected = await _context.Database.ExecuteSqlAsync(
                $"""
                 INSERT INTO "Projects" (
                     "Id", "Title", "Description", "Budget", "PaymentIntentId",
                     "EmployerUserId", "IsActive", "CategoryId"
                 )
                 VALUES (
                     {project.Id},
                     {project.Title},
                     {project.Description},
                     {project.Budget},
                     {project.PaymentIntentId},
                     {project.EmployerUserId},
                     {project.IsActive},
                     {project.CategoryId}
                 )
                 """,
                cancellationToken);

            if (rowsAffected != 1)
            {
                _logger.LogError("Failed to create project. Affected [{rowsAffected}] rows", rowsAffected);
                throw new InvalidOperationException($"Failed to create project. Affected [{rowsAffected}] rows");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to create project. Error: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to create project. Error: {ex.Message}");
        }
    }

    public async Task UpdateFreelancerUserIdAsync(
        Guid id,
        Guid freelancerUserId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var rowsAffected = await _context.Database.ExecuteSqlAsync(
                $"""
                 UPDATE "Projects"
                 SET "FreelancerUserId" = {freelancerUserId},
                 WHERE "Id" = {id}
                 """,
                cancellationToken);

            if (rowsAffected != 1)
            {
                _logger.LogError("Failed to update project freelancer user id. Affected [{rowsAffected}] rows",
                    rowsAffected);
                throw new InvalidOperationException(
                    $"Failed to update project freelancer user id. Affected [{rowsAffected}] rows");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to update project freelancer user id. Error: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to update project freelancer user id. Error: {ex.Message}");
        }
    }

    public async Task UpdateFreelancerUserIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var rowsAffected = await _context.Database.ExecuteSqlAsync(
                $"""
                 UPDATE "Projects"
                 SET "FreelancerUserId" = (
                    SELECT "FreelancerUserId" 
                    FROM "FreelancerApplications"
                    WHERE "ProjectId" = {id.ToString()} AND "Status" = 'Accepted'
                    LIMIT 1)
                 WHERE "Id" = {id.ToString()}
                 """,
                cancellationToken);

            if (rowsAffected != 1)
            {
                _logger.LogError("Failed to set freelancer user id to project. Affected [{rowsAffected}] rows", rowsAffected);
                throw new InvalidOperationException($"Failed to set freelancer user id to project. Affected [{rowsAffected}] rows");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to set freelancer user id to project. Error: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to set freelancer user id to project. Error: {ex.Message}");
        }
    }

    public async Task UpdateAsync(Project project, CancellationToken cancellationToken = default)
    {
        try
        {
            var rowsAffected = await _context.Database.ExecuteSqlAsync(
                $"""
                 UPDATE "Projects"
                 SET 
                     "Title" = {project.Title},
                     "Description" = {project.Description},
                     "Budget" = {project.Budget},
                     "FreelancerUserId" = {project.FreelancerUserId},
                     "CategoryId" = {project.CategoryId}
                 WHERE "Id" = {project.Id.ToString()}
                 """,
                cancellationToken);

            if (rowsAffected != 1)
            {
                _logger.LogError("Failed to update project. Affected [{rowsAffected}] rows", rowsAffected);
                throw new InvalidOperationException($"Failed to update project. Affected [{rowsAffected}] rows");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to update project. Error: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to update project. Error: {ex.Message}");
        }
    }

    public async Task UpdateIsActiveAsync(Guid id, bool isActive, CancellationToken cancellationToken = default)
    {
        try
        {
            var rowsAffected = await _context.Database.ExecuteSqlAsync(
                $"""
                 UPDATE "Projects"
                 SET "IsActive" = {isActive}
                 WHERE "Id" = {id.ToString()}
                 """,
                cancellationToken);

            if (rowsAffected != 1)
            {
                _logger.LogError("Failed to update project is active status. Affected [{rowsAffected}] rows",
                    rowsAffected);
                throw new InvalidOperationException(
                    $"Failed to update project is active status. Affected [{rowsAffected}] rows");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to update project. Error: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to update project. Error: {ex.Message}");
        }
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var rowsAffected = await _context.Database.ExecuteSqlAsync(
                $"""
                 DELETE FROM "Projects" WHERE "Id" = {id.ToString()}
                 """,
                cancellationToken);

            if (rowsAffected != 1)
            {
                _logger.LogError("Failed to delete project. Affected [{rowsAffected}] rows", rowsAffected);
                throw new InvalidOperationException($"Failed to delete project. Affected [{rowsAffected}] rows");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to delete project. Error: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to delete project. Error: {ex.Message}");
        }
    }
}