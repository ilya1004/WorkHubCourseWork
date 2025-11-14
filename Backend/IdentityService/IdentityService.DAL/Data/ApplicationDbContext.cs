using System.Reflection;

namespace IdentityService.DAL.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :
        base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<FreelancerProfile> FreelancerProfiles { get; set; }
    public DbSet<EmployerProfile> EmployerProfiles { get; set; }
    public DbSet<EmployerIndustry> EmployerIndustries { get; set; }
    public DbSet<Cv> Cvs { get; set; }
    public DbSet<CvSkill> CvSkills { get; set; }
    public DbSet<CvLanguage> CvLanguages { get; set; }
    public DbSet<CvWorkExperience> CvWorkExperiences { get; set; }
}