using ProjectsService.Domain.Models;

namespace ProjectsService.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :
        base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ProjectInfo>(entity =>
        {
            entity.HasNoKey();
            entity.ToView("ProjectInfo");

            entity.Property(e => e.ProjectStatus)
                .HasConversion<string>();

            entity.Property(e => e.AcceptanceStatus)
                .HasConversion<string>();
        });
    }

    public DbSet<Category> Categories { get; set; }
    public DbSet<FreelancerApplication> FreelancerApplications { get; set; }
    public DbSet<Lifecycle> Lifecycles { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<Report> Reports { get; set; }
    public DbSet<StarredProject> StarredProjects { get; set; }
}