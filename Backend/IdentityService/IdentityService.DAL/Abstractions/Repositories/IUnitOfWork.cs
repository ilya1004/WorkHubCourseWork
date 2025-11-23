namespace IdentityService.DAL.Abstractions.Repositories;

public interface IUnitOfWork
{
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