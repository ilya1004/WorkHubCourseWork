namespace IdentityService.DAL.Views;

public class EmployerUserModel
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public DateTime RegisteredAt { get; set; }
    public string? ImageUrl { get; set; }
    public string RoleName { get; set; }

    public string CompanyName { get; set; }
    public string? About { get; set; }
    public string? StripeCustomerId { get; set; }
    public Guid? IndustryId { get; set; }
    public string? IndustryName { get; set; }
}