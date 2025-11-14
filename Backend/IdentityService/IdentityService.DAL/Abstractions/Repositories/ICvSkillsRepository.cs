namespace IdentityService.DAL.Abstractions.Repositories;

public interface ICvSkillsRepository
{
    Task CreateAsync(CvSkill cvSkill, CancellationToken cancellationToken = default);
}