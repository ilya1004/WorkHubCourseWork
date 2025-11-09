namespace IdentityService.DAL.Abstractions.Repositories;

public interface IUnitOfWork
{
    public IRepository<EmployerProfile> EmployerProfilesRepository { get; }
    public IFreelancerProfilesRepository FreelancerProfilesRepository { get; }
    public IRepository<CvSkill> FreelancerSkillsRepository { get; }
    public IRepository<EmployerIndustry> EmployerIndustriesRepository { get; }
    public IUsersRepository UsersRepository { get; }
    public IRolesRepository RolesRepository { get; }
    public Task SaveAllAsync(CancellationToken cancellationToken = default);
}