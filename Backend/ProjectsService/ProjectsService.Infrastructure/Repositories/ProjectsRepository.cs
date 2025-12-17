using System.Globalization;
using System.Runtime.CompilerServices;
using AutoMapper;
using ProjectsService.Domain.Enums;
using ProjectsService.Domain.Models;
using ProjectsService.Infrastructure.Data;

namespace ProjectsService.Infrastructure.Repositories;

public class ProjectsRepository : IProjectsRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ProjectsRepository> _logger;
    private readonly IMapper _mapper;

    public ProjectsRepository(
        ApplicationDbContext context,
        ILogger<ProjectsRepository> logger,
        IMapper mapper)
    {
        _context = context;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<Project?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default,
        bool includeRelatedEntities = false,
        bool includeRelatedCollections = false)
    {
        try
        {
            var project = await _context.Projects
                .FromSqlInterpolated($"""
                                      SELECT * FROM "Projects" WHERE "Id" = {id}
                                      """)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            if (project is null || !includeRelatedEntities)
                return project;

            project.Lifecycle = await _context.Lifecycles
                .FromSqlInterpolated($"""
                                      SELECT * FROM "Lifecycles" WHERE "ProjectId" = {id}
                                      """)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            if (project.CategoryId.HasValue)
            {
                project.Category = await _context.Categories
                    .FromSqlInterpolated($"""
                                          SELECT * FROM "Categories" WHERE "Id" = {project.CategoryId.Value}
                                          """)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(cancellationToken);
            }

            if (includeRelatedCollections)
            {
                project.FreelancerApplications = await _context.FreelancerApplications
                    .FromSqlInterpolated($"""
                                          SELECT * FROM "FreelancerApplications" 
                                          WHERE "ProjectId" = {project.Id}
                                          ORDER BY "Id" DESC
                                          """)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);
            }

            return project;
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to get project by id. Error: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to get project by id. Error: {ex.Message}");
        }
    }

    public async Task<IReadOnlyList<ProjectInfo>> PaginatedListAllAsync(
        int offset,
        int limit,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var entities = await _context.Database
                .SqlQuery<ProjectInfoView>($"""
                                            SELECT * FROM "ProjectInfo"
                                            ORDER BY "Id" DESC
                                            LIMIT {limit} OFFSET {offset}
                                            """)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return entities.Select(x => _mapper.Map<ProjectInfo>(x)).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to get projects. Error: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to get projects. Error: {ex.Message}");
        }
    }

    public async Task<IReadOnlyList<Project>> GetByEmployerUserIdAsync(
        Guid employerUserId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Projects
                .FromSqlInterpolated($"""
                                      SELECT * FROM "Projects" 
                                      WHERE "EmployerUserId" = {employerUserId}
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
                .FromSqlInterpolated($"""
                                      SELECT * FROM "Projects" 
                                      WHERE "EmployerUserId" = {employerUserId} AND "Title" = {title}
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

    public async Task<IReadOnlyList<ProjectInfo>> GetByIsActiveAsync(
        bool? isActive,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Database
                .SqlQuery<ProjectInfo>($"""
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

    public async Task<IReadOnlyList<ProjectInfo>> GetFilteredAsync(
        Guid? categoryId = null,
        Guid? employerUserId = null,
        Guid? freelancerUserId = null,
        ProjectStatus? projectStatus = null,
        ProjectAcceptanceStatus? acceptanceStatus = null,
        string? searchTitle = null,
        bool? isActive = null,
        DateTime? updatedAtStartDate = null,
        DateTime? updatedAtEndDate = null,
        decimal? budgetFrom = null,
        decimal? budgetTo = null,
        int offset = 0,
        int limit = 10,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var conditions = new List<string>();

            if (categoryId.HasValue)
                conditions.Add($"\"CategoryId\" = '{categoryId.Value}'");

            if (employerUserId.HasValue)
                conditions.Add($"\"EmployerUserId\" = '{employerUserId.Value}'");

            if (freelancerUserId.HasValue)
                conditions.Add($"\"FreelancerUserId\" = '{freelancerUserId.Value}'");

            if (projectStatus.HasValue)
                conditions.Add($"\"ProjectStatus\" = '{projectStatus.Value}'");

            if (acceptanceStatus.HasValue)
                conditions.Add($"\"AcceptanceStatus\" = '{acceptanceStatus.Value}'");

            if (!string.IsNullOrWhiteSpace(searchTitle))
            {
                var search = searchTitle.Trim().ToLower().Replace("'", "''");
                conditions.Add($"LOWER(\"Title\") LIKE '%{search}%'");
            }

            if (isActive.HasValue)
                conditions.Add($"\"IsActive\" = {isActive.Value.ToString().ToLower()}");

            if (updatedAtStartDate.HasValue)
            {
                var start = updatedAtStartDate.Value.ToString("yyyy-MM-dd HH:mm:ss");
                conditions.Add($"\"UpdatedAt\" >= '{start}'");
            }

            if (updatedAtEndDate.HasValue)
            {
                var endOfDay = updatedAtEndDate.Value.Date.AddDays(1).AddTicks(-1).ToString("yyyy-MM-dd HH:mm:ss");
                conditions.Add($"\"UpdatedAt\" <= '{endOfDay}'");
            }

            if (budgetFrom.HasValue)
                conditions.Add($"\"Budget\" >= {budgetFrom.Value}");

            if (budgetTo.HasValue)
                conditions.Add($"\"Budget\" <= {budgetTo.Value}");

            var whereClause = conditions.Count > 0
                ? "WHERE " + string.Join(" AND ", conditions)
                : string.Empty;

            var sql = $"SELECT * FROM \"ProjectInfo\" {whereClause} ORDER BY \"Id\" DESC LIMIT {limit} OFFSET {offset}";

            var entities = await _context.Database
                .SqlQuery<ProjectInfoView>(FormattableStringFactory.Create(sql))
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return entities.Select(view => _mapper.Map<ProjectInfo>(view)).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get filtered projects with filters.");
            throw new InvalidOperationException("Failed to retrieve projects with filters.", ex);
        }
    }

    public async Task<int> CountByFilteredAsync(
        Guid? categoryId = null,
        Guid? employerUserId = null,
        Guid? freelancerUserId = null,
        ProjectStatus? projectStatus = null,
        ProjectAcceptanceStatus? acceptanceStatus = null,
        string? searchTitle = null,
        bool? isActive = null,
        DateTime? updatedAtStartDate = null,
        DateTime? updatedAtEndDate = null,
        decimal? budgetFrom = null,
        decimal? budgetTo = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var conditions = new List<string>();

            if (categoryId.HasValue)
                conditions.Add($"\"CategoryId\" = '{categoryId.Value}'");

            if (employerUserId.HasValue)
                conditions.Add($"\"EmployerUserId\" = '{employerUserId.Value}'");

            if (freelancerUserId.HasValue)
                conditions.Add($"\"FreelancerUserId\" = '{freelancerUserId.Value}'");

            if (projectStatus.HasValue)
                conditions.Add($"\"ProjectStatus\" = '{projectStatus.Value}'");

            if (acceptanceStatus.HasValue)
                conditions.Add($"\"AcceptanceStatus\" = '{acceptanceStatus.Value}'");

            if (!string.IsNullOrWhiteSpace(searchTitle))
            {
                var search = searchTitle.Trim().ToLower().Replace("'", "''");
                conditions.Add($"LOWER(\"Title\") LIKE '%{search}%'");
            }

            if (isActive.HasValue)
                conditions.Add($"\"IsActive\" = {isActive.Value.ToString().ToLower()}");

            if (updatedAtStartDate.HasValue)
            {
                var start = updatedAtStartDate.Value.ToString("yyyy-MM-dd HH:mm:ss");
                conditions.Add($"\"UpdatedAt\" >= '{start}'");
            }

            if (updatedAtEndDate.HasValue)
            {
                var endOfDay = updatedAtEndDate.Value.Date.AddDays(1).AddTicks(-1).ToString("yyyy-MM-dd HH:mm:ss");
                conditions.Add($"\"UpdatedAt\" <= '{endOfDay}'");
            }

            if (budgetFrom.HasValue)
                conditions.Add($"\"Budget\" >= {budgetFrom.Value}");

            if (budgetTo.HasValue)
                conditions.Add($"\"Budget\" <= {budgetTo.Value}");

            var whereClause = conditions.Count > 0
                ? " WHERE " + string.Join(" AND ", conditions)
                : string.Empty;

            var sql = $"SELECT COUNT(*) AS \"Value\" FROM \"ProjectInfo\"{whereClause}";

            var count = await _context.Database
                .SqlQuery<int>(FormattableStringFactory.Create(sql))
                .SingleAsync(cancellationToken);

            return count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to count projects with filters.");
            throw new InvalidOperationException("Failed to count projects with filters.", ex);
        }
    }

    public async Task CreateAsync(Project project, CancellationToken cancellationToken = default)
    {
        try
        {
            var rowsAffected = await _context.Database.ExecuteSqlInterpolatedAsync(
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
            var rowsAffected = await _context.Database.ExecuteSqlInterpolatedAsync(
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

    public async Task<int> CountAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Database
                .SqlQuery<int>(
                    $"""
                        SELECT COUNT(*) AS "Value" FROM "Projects"
                     """)
                .SingleAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to get projects count. Error: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to get projects count. Error: {ex.Message}");
        }
    }

    public async Task UpdateFreelancerUserIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var rowsAffected = await _context.Database.ExecuteSqlInterpolatedAsync(
                $"""
                 UPDATE "Projects"
                 SET "FreelancerUserId" = (
                    SELECT "FreelancerUserId" 
                    FROM "FreelancerApplications"
                    WHERE "ProjectId" = {id} AND "Status" = 'Accepted'
                    LIMIT 1)
                 WHERE "Id" = {id}
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
            var rowsAffected = await _context.Database.ExecuteSqlInterpolatedAsync(
                $"""
                 UPDATE "Projects"
                 SET 
                     "Title" = {project.Title},
                     "Description" = {project.Description},
                     "Budget" = {project.Budget},
                     "FreelancerUserId" = {project.FreelancerUserId},
                     "CategoryId" = {project.CategoryId}
                 WHERE "Id" = {project.Id}
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

    public async Task UpdatePaymentIntentAsync(Guid id, string paymentIntentId, CancellationToken cancellationToken = default)
    {
        try
        {
            var rowsAffected = await _context.Database.ExecuteSqlInterpolatedAsync(
                $"""
                 UPDATE "Projects"
                 SET "PaymentIntentId" = {paymentIntentId}
                 WHERE "Id" = {id}
                 """,
                cancellationToken);

            if (rowsAffected != 1)
            {
                _logger.LogError("Failed to update project payment intent id. Affected [{rowsAffected}] rows", rowsAffected);
                throw new InvalidOperationException($"Failed to update project payment intent id. Affected [{rowsAffected}] rows");
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
            var rowsAffected = await _context.Database.ExecuteSqlInterpolatedAsync(
                $"""
                 UPDATE "Projects"
                 SET "IsActive" = {isActive}
                 WHERE "Id" = {id}
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
            var rowsAffected = await _context.Database.ExecuteSqlInterpolatedAsync(
                $"""
                 DELETE FROM "Projects" WHERE "Id" = {id}
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