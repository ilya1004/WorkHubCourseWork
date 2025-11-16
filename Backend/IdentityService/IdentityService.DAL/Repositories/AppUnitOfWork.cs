namespace IdentityService.DAL.Repositories;

public class AppUnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public IEmployerProfilesRepository EmployerProfilesRepository { get; }
    public IFreelancerProfilesRepository FreelancerProfilesRepository { get; }
    public ICvSkillsRepository CvSkillsRepository { get; }
    public IEmployerIndustriesRepository EmployerIndustriesRepository { get; }
    public IUsersRepository UsersRepository { get; }
    public IRolesRepository RolesRepository { get; }

    public AppUnitOfWork(
        ApplicationDbContext context,
        IEmployerProfilesRepository employerProfilesRepository,
        IFreelancerProfilesRepository freelancerProfilesRepository,
        ICvSkillsRepository cvSkillsRepository,
        IEmployerIndustriesRepository employerIndustriesRepository,
        IUsersRepository usersRepository,
        IRolesRepository rolesRepository)
    {
        _context = context;
        EmployerProfilesRepository = employerProfilesRepository;
        FreelancerProfilesRepository = freelancerProfilesRepository;
        CvSkillsRepository = cvSkillsRepository;
        EmployerIndustriesRepository = employerIndustriesRepository;
        UsersRepository = usersRepository;
        RolesRepository = rolesRepository;
    }
}