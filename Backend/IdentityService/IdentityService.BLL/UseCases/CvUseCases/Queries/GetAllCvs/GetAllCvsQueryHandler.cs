using IdentityService.BLL.Models;

namespace IdentityService.BLL.UseCases.CvUseCases.Queries.GetAllCvs;

public class GetAllCvsQueryHandler : IRequestHandler<GetAllCvsQuery, PaginatedResultModel<Cv>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetAllCvsQueryHandler> _logger;

    public GetAllCvsQueryHandler(IUnitOfWork unitOfWork, ILogger<GetAllCvsQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<PaginatedResultModel<Cv>> Handle(GetAllCvsQuery request, CancellationToken cancellationToken)
    {
        var offset = (request.PageNo - 1) * request.PageSize;

        var cvs = await _unitOfWork.CvsRepository.GetAllPaginatedAsync(
            offset,
            request.PageSize,
            cancellationToken);

        var industriesCount = await _unitOfWork.EmployerIndustriesRepository.CountAllAsync(cancellationToken);

        return new PaginatedResultModel<Cv>
        {
            Items = cvs.ToList(),
            TotalCount = industriesCount,
            PageNo = request.PageNo,
            PageSize = request.PageSize
        };
    }
}