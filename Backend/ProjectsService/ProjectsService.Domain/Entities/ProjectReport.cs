using ProjectsService.Domain.Enums;
using ProjectsService.Domain.Primitives;

namespace ProjectsService.Domain.Entities;

public class ProjectReport : Entity
{
    public string? Description { get; set; }
    public ReportStatus Status { get; set; }
    public Guid ProjectId { get; set; }
    public Project? Project { get; set; }
    public Guid ReporterUserId { get; set; }
    public Guid? ReviewerUserId { get; set; }
}