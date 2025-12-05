namespace ProjectsService.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :
        base(options) { }

    public DbSet<Category> Categories { get; set; }
    public DbSet<FreelancerApplication> FreelancerApplications { get; set; }
    public DbSet<Lifecycle> Lifecycles { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<Report> Reports { get; set; }
    public StarredProject StarredProject { get; set; }
}