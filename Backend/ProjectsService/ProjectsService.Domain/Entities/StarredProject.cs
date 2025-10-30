using ProjectsService.Domain.Primitives;

namespace ProjectsService.Domain.Entities;

public class StarredProject : Entity
{
    public Guid ProjectId { get; set; }
    public Project? Project { get; set; }
    public Guid FreelancerUserId { get; set; }
}