using IdentityService.DAL.Primitives;

namespace IdentityService.DAL.Entities;

public class CvSkill : Entity
{
    public string Name { get; set; }
    public string NormalizedName { get; set; }
    public int? ExperienceInYears { get; set; }
    public ICollection<FreelancerProfile> FreelancerProfiles { get; set; }
}