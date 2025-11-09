namespace IdentityService.BLL.UseCases.UserUseCases.Commands.RegisterFreelancer;

public sealed record RegisterFreelancerCommand(
    string Nickname,
    string FirstName,
    string LastName,
    string Email,
    string Password) : IRequest;