namespace IdentityService.DAL.Abstractions.Repositories;

public interface IEmployerIndustriesRepository
{
    Task<EmployerIndustry?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<EmployerIndustry?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EmployerIndustry>> GetAllPaginatedAsync(
        int offset,
        int limit,
        CancellationToken cancellationToken = default);
    Task<int> CountAllAsync(CancellationToken cancellationToken = default);
    Task CreateAsync(EmployerIndustry employerIndustry, CancellationToken cancellationToken = default);
    Task UpdateAsync(EmployerIndustry employerIndustry, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}