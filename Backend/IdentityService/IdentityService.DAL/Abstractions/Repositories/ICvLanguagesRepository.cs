namespace IdentityService.DAL.Abstractions.Repositories;

public interface ICvLanguagesRepository
{
    Task<CvLanguage?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CvLanguage>> GetByCvIdAsync(Guid cvId, CancellationToken cancellationToken = default);
    Task AddAsync(CvLanguage language, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task DeleteByCvIdAsync(Guid cvId, CancellationToken cancellationToken = default);
}