using IdentityService.DAL.Primitives;

namespace IdentityService.DAL.Entities;

public class Role : Entity
{
    public string Name { get; set; }
    public string NormalizedName { get; set; }
}