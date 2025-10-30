using System.ComponentModel;

namespace ProjectsService.Domain.Enums;

public enum ReportStatus
{
    [Description("Sent")]
    Sent = 0,

    [Description("Reviewed")]
    Reviewed = 1,

    [Description("Rejected")]
    Rejected = 2
}