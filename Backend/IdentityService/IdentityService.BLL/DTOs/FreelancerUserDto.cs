namespace IdentityService.BLL.DTOs;

public record FreelancerUserDto
{
    public Guid Id { get; init; }
    public string Nickname { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string? About { get; init; }
    public string Email { get; init; }
    public DateTime RegisteredAt { get; init; }
    public string? StripeAccountId { get; init; }
    public string? ImageUrl { get; init; }
    public string RoleName { get; init; }
}