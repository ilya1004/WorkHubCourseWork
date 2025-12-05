using System.ComponentModel;

namespace ProjectsService.Domain.Enums;

public enum ApplicationStatus
{
    [Description("Pending")] Pending = 0,
    [Description("Accepted")] Accepted = 1,
    [Description("Rejected")] Rejected = 2
}