using IdentityService.DAL.Enums;
using IdentityService.DAL.Primitives;

namespace IdentityService.DAL.Entities;

public class CvLanguage : Entity
{
    public string Name { get; set; }
    public CvLanguageLevel Level { get; set; }
    public Guid CvId { get; set; }
    public Cv? Cv { get; set; }
}