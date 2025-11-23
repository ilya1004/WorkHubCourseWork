namespace IdentityService.DAL.Abstractions.Repositories;

public interface ICvWorkExperiencesRepository
{
    Task<CvWorkExperience?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CvWorkExperience>> GetByCvIdAsync(Guid cvId, CancellationToken cancellationToken = default);
    Task AddAsync(CvWorkExperience experience, CancellationToken cancellationToken = default);
    Task UpdateAsync(CvWorkExperience experience, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task DeleteByCvIdAsync(Guid cvId, CancellationToken cancellationToken = default);
}