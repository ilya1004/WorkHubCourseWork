namespace IdentityService.BLL.DTOs;

public record CvWorkExperienceDto
{
    public Guid Id { get; set; }
    public string UserSpecialization { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Responsibilities { get; set; }
    public Guid CvId { get; set; }
}