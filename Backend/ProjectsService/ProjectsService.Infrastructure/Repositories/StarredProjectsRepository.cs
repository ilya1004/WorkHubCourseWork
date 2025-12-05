using ProjectsService.Infrastructure.Data;

namespace ProjectsService.Infrastructure.Repositories;

public class StarredProjectsRepository
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


}