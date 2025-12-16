namespace IdentityService.BLL.DTOs;

public record CvDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string UserSpecialization { get; set; }
    public string? UserEducation { get; set; }
    public bool IsPublic { get; set; }
    public Guid FreelancerUserId { get; set; }
    public List<CvLanguageDto> CvLanguages { get; set; }
    public List<CvSkillDto> CvSkills { get; set; }
    public List<CvWorkExperienceDto> CvWorkExperiences { get; set; }
}