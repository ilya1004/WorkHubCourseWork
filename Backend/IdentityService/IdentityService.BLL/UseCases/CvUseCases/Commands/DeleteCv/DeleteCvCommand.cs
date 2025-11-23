namespace IdentityService.BLL.UseCases.CvUseCases.Commands.DeleteCv;

public sealed record DeleteCvCommand(Guid Id) : IRequest;