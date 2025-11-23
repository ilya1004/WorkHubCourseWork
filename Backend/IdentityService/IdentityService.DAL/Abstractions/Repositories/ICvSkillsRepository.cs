namespace IdentityService.DAL.Abstractions.Repositories;

public interface ICvSkillsRepository
{
    Task<CvSkill?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CvSkill>> GetByCvIdAsync(Guid cvId, CancellationToken cancellationToken = default);
    Task AddAsync(CvSkill skill, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task DeleteByCvIdAsync(Guid cvId, CancellationToken cancellationToken = default);
}