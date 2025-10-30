using ProjectsService.Domain.Primitives;

namespace ProjectsService.Domain.Entities;

public class Project : Entity
{
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal Budget { get; set; }
    public string? PaymentIntentId { get; set; }
    public Guid EmployerUserId { get; set; }
    public Guid? FreelancerUserId { get; set; }
    public Lifecycle Lifecycle { get; set; }
    public Guid? CategoryId { get; set; }
    public Category? Category { get; set; }
    public ICollection<FreelancerApplication> FreelancerApplications { get; set; }
    public ICollection<ProjectReport> Reports { get; set; }
}
