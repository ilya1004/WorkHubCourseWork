using ProjectsService.Domain.Entities;
using ProjectsService.Domain.Enums;

namespace ProjectsService.Domain.Abstractions.Data;

public interface IReportsRepository
{
    Task<Report?> GetByUserId(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Report>> GetAllAsync(CancellationToken cancellationToken = default);
    Task CreateAsync(Report report, CancellationToken cancellationToken = default);

    Task UpdateStatusAsync(
        Guid reportId,
        ReportStatus reportStatus,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}