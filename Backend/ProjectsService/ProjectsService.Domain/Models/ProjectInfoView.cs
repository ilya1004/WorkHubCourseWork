namespace ProjectsService.Domain.Models;

public class ProjectInfoView
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal Budget { get; set; }
    public string? PaymentIntentId { get; set; }
    public Guid EmployerUserId { get; set; }
    public Guid? FreelancerUserId { get; set; }
    public Guid? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime ApplicationsStartDate { get; set; }
    public DateTime ApplicationsDeadline { get; set; }
    public DateTime WorkStartDate { get; set; }
    public DateTime WorkDeadline { get; set; }
    public string AcceptanceStatus { get; set; }
    public string ProjectStatus { get; set; }
}