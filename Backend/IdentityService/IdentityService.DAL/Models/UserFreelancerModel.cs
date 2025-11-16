namespace IdentityService.DAL.Models;

public class UserFreelancerModel
{
    public Guid Id { get; set; }
    public DateTime RegisteredAt { get; set; }
    public string? ImageUrl { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
    public bool IsEmailConfirmed { get; set; }
    public bool IsActive { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Nickname { get; set; }
    public string? About { get; set; }
    public string? StripeAccountId { get; set; }
    public Guid RoleId { get; set; }
    public string RoleName { get; set; }
}