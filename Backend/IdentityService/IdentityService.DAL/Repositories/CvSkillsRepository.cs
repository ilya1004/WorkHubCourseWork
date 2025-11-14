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

    public async Task CreateAsync(CvSkill cvSkill, CancellationToken cancellationToken = default)
    {
        try
        {
            var rowsAffected = await _context.Database.ExecuteSqlAsync(
                $"""
                 INSERT INTO "CvSkills" ("Id", "Name", "ExperienceInYears", "CvId")
                 VALUES ({cvSkill.Id}, {cvSkill.Name}, {cvSkill.ExperienceInYears}, {cvSkill.CvId})
                 """,
                cancellationToken);

            if (rowsAffected != 1)
            {
                _logger.LogError("Failed to create cv skill. Affected [{rowsAffected}] rows", rowsAffected);
                throw new InvalidOperationException($"Failed to create cv skill. Affected [{rowsAffected}] rows");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to create cv skill. Error: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to create cv skill. Error: {ex.Message}");
        }
    }
}