namespace IdentityService.DAL.Views;

public class FreelancerUserModel
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public DateTime RegisteredAt { get; set; }
    public string? ImageUrl { get; set; }
    public string RoleName { get; set; }

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Nickname { get; set; }
    public string? About { get; set; }
    public string? StripeAccountId { get; set; }
}