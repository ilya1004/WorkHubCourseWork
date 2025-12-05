namespace IdentityService.DAL.Repositories;

public class AppUnitOfWork : IUnitOfWork
{
    public AppUnitOfWork(
        IEmployerProfilesRepository employerProfilesRepository,
        IFreelancerProfilesRepository freelancerProfilesRepository,
        ICvSkillsRepository cvSkillsRepository,
        IEmployerIndustriesRepository employerIndustriesRepository,
        IUsersRepository usersRepository,
        IRolesRepository rolesRepository,
        ICvsRepository cvsRepository,
        ICvLanguagesRepository cvLanguagesRepository,
        ICvWorkExperiencesRepository cvWorkExperiencesRepository)
    {
        EmployerProfilesRepository = employerProfilesRepository;
        FreelancerProfilesRepository = freelancerProfilesRepository;
        CvSkillsRepository = cvSkillsRepository;
        EmployerIndustriesRepository = employerIndustriesRepository;
        UsersRepository = usersRepository;
        RolesRepository = rolesRepository;
        CvsRepository = cvsRepository;
        CvLanguagesRepository = cvLanguagesRepository;
        CvWorkExperiencesRepository = cvWorkExperiencesRepository;
    }

    public IEmployerProfilesRepository EmployerProfilesRepository { get; }
    public IFreelancerProfilesRepository FreelancerProfilesRepository { get; }
    public ICvsRepository CvsRepository { get; }
    public ICvSkillsRepository CvSkillsRepository { get; }
    public ICvLanguagesRepository CvLanguagesRepository { get; }
    public ICvWorkExperiencesRepository CvWorkExperiencesRepository { get; }
    public IEmployerIndustriesRepository EmployerIndustriesRepository { get; }
    public IUsersRepository UsersRepository { get; }
    public IRolesRepository RolesRepository { get; }
}