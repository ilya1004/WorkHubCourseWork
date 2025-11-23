using IdentityService.DAL.Enums;

namespace IdentityService.BLL.DTOs;

public record CvLanguageDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public CvLanguageLevel Level { get; set; }
    public Guid CvId { get; set; }
}