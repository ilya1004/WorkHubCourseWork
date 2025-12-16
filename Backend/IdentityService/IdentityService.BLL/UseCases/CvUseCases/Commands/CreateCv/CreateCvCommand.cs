using IdentityService.BLL.DTOs;

namespace IdentityService.BLL.UseCases.CvUseCases.Commands.CreateCv;

public sealed record CreateCvCommand(CvDto CvDto) : IRequest<Cv>;