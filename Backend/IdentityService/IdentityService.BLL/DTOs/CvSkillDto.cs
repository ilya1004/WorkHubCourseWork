namespace IdentityService.BLL.DTOs;

public record CvSkillDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int ExperienceInYears { get; set; }
    public Guid CvId { get; set; }
}