using IdentityService.DAL.Primitives;

namespace IdentityService.DAL.Entities;

public class Cv : Entity
{
    public string Title { get; set; }
    public string UserSpecialization { get; set; }
    public string? UserEducation { get; set; }
    public bool IsPublic { get; set; }
    public Guid FreelancerUserId { get; set; }
    public FreelancerProfile FreelancerUserProfile { get; set; }
    public ICollection<CvLanguage> Languages { get; set; }
    public ICollection<CvSkill> Skills { get; set; }
    public ICollection<CvWorkExperience> WorkExperiences { get; set; }
}