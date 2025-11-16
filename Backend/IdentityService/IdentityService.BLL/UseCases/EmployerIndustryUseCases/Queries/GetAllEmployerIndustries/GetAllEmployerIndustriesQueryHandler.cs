using IdentityService.BLL.Models;

namespace IdentityService.BLL.UseCases.EmployerIndustryUseCases.Queries.GetAllEmployerIndustries;

public class GetAllEmployerIndustriesQueryHandler : IRequestHandler<GetAllEmployerIndustriesQuery, PaginatedResultModel<EmployerIndustry>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetAllEmployerIndustriesQueryHandler> _logger;

    public GetAllEmployerIndustriesQueryHandler(IUnitOfWork unitOfWork,
        ILogger<GetAllEmployerIndustriesQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<PaginatedResultModel<EmployerIndustry>> Handle(GetAllEmployerIndustriesQuery request,
        CancellationToken cancellationToken)
    {
        var offset = (request.PageNo - 1) * request.PageSize;

        var industries = await _unitOfWork.EmployerIndustriesRepository.GetAllPaginatedAsync(
            offset,
            request.PageSize,
            cancellationToken);

        var industriesCount = await _unitOfWork.EmployerIndustriesRepository.CountAllAsync(cancellationToken);

        return new PaginatedResultModel<EmployerIndustry>
        {
            Items = industries.ToList(),
            TotalCount = industriesCount,
            PageNo = request.PageNo,
            PageSize = request.PageSize
        };
    }
}