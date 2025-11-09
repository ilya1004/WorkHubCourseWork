using IdentityService.DAL.Primitives;

namespace IdentityService.DAL.Entities;

public class User : Entity
{
    public DateTime RegisteredAt { get; set; }
    public string? ImageUrl { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
    public bool IsEmailConfirmed { get; set; }
    public bool IsActive { get; set; }
    public FreelancerProfile? FreelancerProfile { get; set; }
    public EmployerProfile? EmployerProfile { get; set; }
    public Guid RoleId { get; set; }
    public Role? Role { get; set; }
}