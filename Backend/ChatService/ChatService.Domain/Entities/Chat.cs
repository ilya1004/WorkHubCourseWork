namespace ChatService.Domain.Entities;

public class Chat
{
    public string Id { get; set; }
    public Guid EmployerUserId { get; set; }
    public Guid FreelancerUserId { get; set; }
    public Guid ProjectId { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}