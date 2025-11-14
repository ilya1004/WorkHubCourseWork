namespace IdentityService.DAL.Abstractions.Repositories;

public interface IEmployerIndustriesRepository
{
    Task<EmployerIndustry?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<EmployerIndustry?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task CreateAsync(EmployerIndustry employerIndustry, CancellationToken cancellationToken = default);
}