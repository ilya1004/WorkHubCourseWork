namespace IdentityService.BLL.DTOs;

public record FreelancerProfileDto(
    string FirstName,
    string LastName,
    string Nickname,
    string? About);