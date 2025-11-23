namespace IdentityService.DAL.Repositories;

public class CvLanguagesRepository : ICvLanguagesRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CvLanguagesRepository> _logger;

    public CvLanguagesRepository(ApplicationDbContext context, ILogger<CvLanguagesRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<CvLanguage?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Set<CvLanguage>()
                .FromSql($"""
                          SELECT * FROM "CvLanguages" WHERE "Id" = {id.ToString()}
                          """)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get CvLanguage by id {LanguageId}", id);
            throw new InvalidOperationException($"Failed to get CvLanguage {id}", ex);
        }
    }

    public async Task<IReadOnlyList<CvLanguage>> GetByCvIdAsync(Guid cvId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Set<CvLanguage>()
                .FromSql($"""
                          SELECT * FROM "CvLanguages" WHERE "CvId" = {cvId.ToString()} ORDER BY "Name"
                          """)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get CvLanguages for Cv {CvId}", cvId);
            throw new InvalidOperationException($"Failed to get CvLanguages for Cv {cvId}", ex);
        }
    }

    public async Task AddAsync(CvLanguage language, CancellationToken cancellationToken = default)
    {
        try
        {
            var rows = await _context.Database.ExecuteSqlAsync(
                $"""
                 INSERT INTO "CvLanguages" ("Id", "Name", "Level", "CvId")
                 VALUES ({language.Id}, {language.Name}, {(int)language.Level}, {language.CvId})
                 """, cancellationToken);

            if (rows != 1)
            {
                throw new InvalidOperationException($"CvLanguage creation affected {rows} rows");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add CvLanguage {LanguageId}", language.Id);
            throw;
        }
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var rows = await _context.Database.ExecuteSqlAsync(
                $"""
                 DELETE FROM "CvLanguages" WHERE "Id" = {id}
                 """, cancellationToken);

            if (rows != 1)
            {
                throw new InvalidOperationException($"CvLanguage deletion affected {rows} rows");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete CvLanguage {LanguageId}", id);
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