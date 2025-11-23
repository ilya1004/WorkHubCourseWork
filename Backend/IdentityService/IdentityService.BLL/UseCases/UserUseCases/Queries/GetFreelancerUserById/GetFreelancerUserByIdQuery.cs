using IdentityService.BLL.DTOs;

namespace IdentityService.BLL.UseCases.UserUseCases.Queries.GetFreelancerUserById;

public sealed record GetFreelancerUserByIdQuery(Guid Id) : IRequest<FreelancerUserDto>;