namespace IdentityService.DAL.Repositories;

public class CvSkillsRepository : ICvSkillsRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CvSkillsRepository> _logger;

    public CvSkillsRepository(ApplicationDbContext context, ILogger<CvSkillsRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<CvSkill?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Set<CvSkill>()
                .FromSql($"""
                          SELECT * FROM "CvSkills" WHERE "Id" = {id.ToString()}
                          """)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get CvSkill by id {SkillId}", id);
            throw new InvalidOperationException($"Failed to get CvSkill {id}", ex);
        }
    }

    public async Task<IReadOnlyList<CvSkill>> GetByCvIdAsync(Guid cvId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Set<CvSkill>()
                .FromSql($"""
                          SELECT * FROM "CvSkills" WHERE "CvId" = {cvId.ToString()} ORDER BY "Name"
                          """)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get CvSkills for Cv {CvId}", cvId);
            throw new InvalidOperationException($"Failed to get CvSkills for Cv {cvId}", ex);
        }
    }

    public async Task AddAsync(CvSkill skill, CancellationToken cancellationToken = default)
    {
        try
        {
            var rows = await _context.Database.ExecuteSqlAsync(
                $"""
                 INSERT INTO "CvSkills" ("Id", "Name", "ExperienceInYears", "CvId")
                 VALUES ({skill.Id}, {skill.Name}, {skill.ExperienceInYears}, {skill.CvId})
                 """, cancellationToken);

            if (rows != 1)
            {
                throw new InvalidOperationException($"CvSkill creation affected {rows} rows");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add CvSkill {SkillId}", skill.Id);
            throw;
        }
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var rows = await _context.Database.ExecuteSqlAsync(
                $"""
                 DELETE FROM "CvSkills" WHERE "Id" = {id}
                 """, cancellationToken);

            if (rows != 1)
            {
                throw new InvalidOperationException($"CvSkill deletion affected {rows} rows");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete CvSkill {SkillId}", id);
            throw;
        }
    }

    public async Task DeleteByCvIdAsync(Guid cvId, CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.Database.ExecuteSqlAsync(
                $"""
                 DELETE FROM "CvSkills" WHERE "CvId" = {cvId}
                 """, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete languages for CV {CvId}", cvId);
            throw;
        }
    }
}