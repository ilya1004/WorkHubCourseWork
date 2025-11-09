namespace IdentityService.API.Contracts.UserContracts;

public sealed record RegisterFreelancerRequest(
    string Nickname,
    string FirstName,
    string LastName,
    string Email,
    string Password);