namespace IdentityService.DAL.Abstractions.Repositories;

public interface IUnitOfWork
{
    public IEmployerProfilesRepository EmployerProfilesRepository { get; }
    public IFreelancerProfilesRepository FreelancerProfilesRepository { get; }
    public ICvSkillsRepository CvSkillsRepository { get; }
    public IEmployerIndustriesRepository EmployerIndustriesRepository { get; }
    public IUsersRepository UsersRepository { get; }
    public IRolesRepository RolesRepository { get; }
    public Task SaveAllAsync(CancellationToken cancellationToken = default);
}