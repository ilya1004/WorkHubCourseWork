namespace IdentityService.BLL.UseCases.CvUseCases.Queries.GetCvById;

public record GetCvByIdQuery(Guid Id) : IRequest<Cv>;