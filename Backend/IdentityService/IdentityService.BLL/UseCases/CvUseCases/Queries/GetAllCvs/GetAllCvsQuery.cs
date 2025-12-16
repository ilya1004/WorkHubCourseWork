using IdentityService.BLL.Models;

namespace IdentityService.BLL.UseCases.CvUseCases.Queries.GetAllCvs;

public record GetAllCvsQuery(int PageNo = 1, int PageSize = 10) : IRequest<PaginatedResultModel<Cv>>;