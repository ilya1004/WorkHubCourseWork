using System.ComponentModel;

namespace ProjectsService.Domain.Enums;

public enum ProjectAcceptanceStatus
{
    [Description("None")] None = 0,
    [Description("Requested")] Requested = 1,
    [Description("Accepted")] Accepted = 2
}