namespace IdentityService.DAL.Repositories;

public class CvsRepository : ICvsRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CvsRepository> _logger;

    public CvsRepository(ApplicationDbContext context, ILogger<CvsRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Cv?> GetByIdAsync(Guid cvId, CancellationToken cancellationToken = default, bool includeRelated = false)
    {
        try
        {
            var cv = await _context.Cvs
                .FromSqlInterpolated($"""
                          SELECT * FROM "Cvs" WHERE "Id" = {cvId.ToString()}
                          """)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            if (cv is null || !includeRelated)
            {
                return cv;
            }

            cv.WorkExperiences = await _context.Set<CvWorkExperience>()
                .FromSqlInterpolated($"""
                          SELECT * FROM "CvWorkExperiences" WHERE "CvId" = {cvId.ToString()}
                          """)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            cv.Skills = await _context.Set<CvSkill>()
                .FromSqlInterpolated($"""
                          SELECT * FROM "CvSkills" WHERE "CvId" = {cvId.ToString()}
                          """)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            cv.Languages = await _context.Set<CvLanguage>()
                .FromSqlInterpolated($"""
                          SELECT * FROM "CvLanguages" WHERE "CvId" = {cvId.ToString()}
                          """)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return cv;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get CV by id {CvId}", cvId);
            throw new InvalidOperationException($"Failed to get CV by id {cvId}", ex);
        }
    }

    public async Task<IReadOnlyList<Cv>> GetByFreelancerIdAsync(Guid freelancerUserId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Cvs
                .FromSqlInterpolated($"""
                          SELECT * FROM "Cvs" 
                          WHERE "FreelancerUserId" = {freelancerUserId.ToString()}
                          ORDER BY "Id"
                          """)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get CVs for freelancer {FreelancerId}", freelancerUserId);
            throw new InvalidOperationException($"Failed to get CVs for freelancer {freelancerUserId}", ex);
        }
    }

    public async Task<Cv?> GetPublicByIdAsync(Guid cvId, CancellationToken cancellationToken = default)
    {
        try
        {
            var cv = await _context.Cvs
                .FromSqlInterpolated($"""
                          SELECT * FROM "Cvs" 
                          WHERE "Id" = {cvId.ToString()} AND "IsPublic" = true
                          """)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            if (cv is null)
            {
                return null;
            }

            cv.WorkExperiences = await _context.Set<CvWorkExperience>()
                .FromSqlInterpolated($"""
                          SELECT * FROM "CvWorkExperiences" WHERE "CvId" = {cvId.ToString()}
                          """)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            cv.Skills = await _context.Set<CvSkill>()
                .FromSqlInterpolated($"""
                          SELECT * FROM "CvSkills" WHERE "CvId" = {cvId.ToString()}
                          """)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            cv.Languages = await _context.Set<CvLanguage>()
                .FromSqlInterpolated($"""
                          SELECT * FROM "CvLanguages" WHERE "CvId" = {cvId.ToString()}
                          """)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return cv;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get public CV {CvId}", cvId);
            throw new InvalidOperationException($"Failed to get public CV {cvId}", ex);
        }
    }

    public async Task CreateAsync(Cv cv, CancellationToken cancellationToken = default)
    {
        try
        {
            var rows = await _context.Database.ExecuteSqlInterpolatedAsync(
                $"""
                 INSERT INTO "Cvs" ("Id", "Title", "UserSpecialization", "UserEducation", "IsPublic", "FreelancerUserId")
                 VALUES ({cv.Id}, {cv.Title}, {cv.UserSpecialization}, {cv.UserEducation}, {cv.IsPublic}, {cv.FreelancerUserId})
                 """, cancellationToken);

            if (rows != 1)
            {
                throw new InvalidOperationException($"Failed to create CV. Affected rows: {rows}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create CV {CvId}", cv.Id);
            throw;
        }
    }

    public async Task UpdateAsync(Cv cv, CancellationToken cancellationToken = default)
    {
        try
        {
            var rows = await _context.Database.ExecuteSqlInterpolatedAsync(
                $"""
                 UPDATE "Cvs"
                 SET "Title" = {cv.Title},
                     "UserSpecialization" = {cv.UserSpecialization},
                     "UserEducation" = {cv.UserEducation},
                     "IsPublic" = {cv.IsPublic}
                 WHERE "Id" = {cv.Id}
                 """, cancellationToken);

            if (rows != 1)
            {
                throw new InvalidOperationException($"Failed to update CV {cv.Id}. Affected rows: {rows}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update CV {CvId}", cv.Id);
            throw;
        }
    }

    public async Task DeleteAsync(Guid cvId, CancellationToken cancellationToken = default)
    {
        try
        {
            var rows = await _context.Database.ExecuteSqlInterpolatedAsync(
                $"""
                 DELETE FROM "Cvs" WHERE "Id" = {cvId}
                 """, cancellationToken);

            if (rows != 1)
            {
                throw new InvalidOperationException($"Failed to delete CV {cvId}. Affected rows: {rows}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete CV {CvId}", cvId);
            throw;
        }
    }
}