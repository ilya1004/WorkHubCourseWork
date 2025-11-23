using IdentityService.BLL.DTOs;

namespace IdentityService.BLL.UseCases.UserUseCases.Commands.UpdateFreelancerProfile;

public sealed record UpdateFreelancerProfileCommand(
    FreelancerProfileDto FreelancerProfile,
    bool ResetImage,
    Stream? FileStream,
    string? ContentType) : IRequest;