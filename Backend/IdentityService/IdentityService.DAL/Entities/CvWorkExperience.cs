using IdentityService.DAL.Primitives;

namespace IdentityService.DAL.Entities;

public class CvWorkExperience : Entity
{
    public string UserSpecialization { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public string? Responsibilities { get; set; }
    public Guid CvId { get; set; }
    public Cv? Cv { get; set; }
}