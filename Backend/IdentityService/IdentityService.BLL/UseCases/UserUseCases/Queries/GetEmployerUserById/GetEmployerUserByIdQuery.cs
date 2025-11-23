using IdentityService.BLL.DTOs;

namespace IdentityService.BLL.UseCases.UserUseCases.Queries.GetEmployerUserById;

public sealed record GetEmployerUserByIdQuery(Guid Id) : IRequest<EmployerUserDto>;