namespace IdentityService.DAL.Repositories;

public class CvWorkExperiencesRepository : ICvWorkExperiencesRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CvWorkExperiencesRepository> _logger;

    public CvWorkExperiencesRepository(ApplicationDbContext context, ILogger<CvWorkExperiencesRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<CvWorkExperience?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<CvWorkExperience>()
            .FromSqlInterpolated($"""
                      SELECT * FROM "CvWorkExperiences" WHERE "Id" = {id.ToString()}
                      """)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CvWorkExperience>> GetByCvIdAsync(Guid cvId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<CvWorkExperience>()
            .FromSqlInterpolated($"""
                      SELECT * FROM "CvWorkExperiences" WHERE "CvId" = {cvId.ToString()} ORDER BY "StartDate" DESC
                      """)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(CvWorkExperience experience, CancellationToken cancellationToken = default)
    {
        try
        {
            var rows = await _context.Database.ExecuteSqlInterpolatedAsync(
                $"""
                 INSERT INTO "CvWorkExperiences" ("Id", "UserSpecialization", "StartDate", "EndDate", "Responsibilities", "CvId")
                 VALUES ({experience.Id}, {experience.UserSpecialization}, {experience.StartDate:yyyy-MM-dd},
                         {experience.EndDate:yyyy-MM-dd}, {experience.Responsibilities}, {experience.CvId})
                 """, cancellationToken);

            if (rows != 1)
            {
                throw new InvalidOperationException("Failed to add work experience");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add work experience {Id}", experience.Id);
            throw;
        }
    }

    public async Task UpdateAsync(CvWorkExperience experience, CancellationToken cancellationToken = default)
    {
        try
        {
            var rows = await _context.Database.ExecuteSqlInterpolatedAsync(
                $"""
                 UPDATE "CvWorkExperiences"
                 SET "UserSpecialization" = {experience.UserSpecialization},
                     "StartDate" = {experience.StartDate:yyyy-MM-dd},
                     "EndDate" = {experience.EndDate:yyyy-MM-dd},
                     "Responsibilities" = {experience.Responsibilities}
                 WHERE "Id" = {experience.Id}
                 """, cancellationToken);

            if (rows != 1)
            {
                throw new InvalidOperationException("Failed to update work experience");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update work experience {Id}", experience.Id);
            throw;
        }
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var rows = await _context.Database.ExecuteSqlInterpolatedAsync(
                $"""
                 DELETE FROM "CvWorkExperiences" WHERE "Id" = {id}
                 """, cancellationToken);

            if (rows != 1)
            {
                throw new InvalidOperationException("Failed to delete work experience");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete work experience {Id}", id);
            throw;
        }
    }

    public async Task DeleteByCvIdAsync(Guid cvId, CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.Database.ExecuteSqlInterpolatedAsync(
                $"""
                 DELETE FROM "CvWorkExperiences" WHERE "CvId" = {cvId}
                 """, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete languages for CV {CvId}", cvId);
            throw;
        }
    }
}