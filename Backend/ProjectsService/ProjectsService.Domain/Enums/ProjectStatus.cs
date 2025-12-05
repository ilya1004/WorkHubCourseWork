using System.ComponentModel;

namespace ProjectsService.Domain.Enums;

public enum ProjectStatus
{
    [Description("Published")] Published = 0,
    [Description("AcceptingApplications")] AcceptingApplications = 1,
    [Description("WaitingForWorkStart")] WaitingForWorkStart = 2,
    [Description("InProgress")] InProgress = 3,
    [Description("PendingForReview")] PendingForReview = 4,
    [Description("Completed")] Completed = 5,
    [Description("Expired")] Expired = 6,
    [Description("Cancelled")] Cancelled = 7
}