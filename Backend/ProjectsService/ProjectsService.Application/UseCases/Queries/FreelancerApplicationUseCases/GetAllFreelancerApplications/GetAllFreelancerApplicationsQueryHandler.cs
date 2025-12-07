using ProjectsService.Application.Models;

namespace ProjectsService.Application.UseCases.Queries.FreelancerApplicationUseCases.GetAllFreelancerApplications;

public class GetAllFreelancerApplicationsQueryHandler : IRequestHandler<GetAllFreelancerApplicationsQuery,
    PaginatedResultModel<FreelancerApplication>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllFreelancerApplicationsQueryHandler(
        IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PaginatedResultModel<FreelancerApplication>> Handle(
        GetAllFreelancerApplicationsQuery request,
        CancellationToken cancellationToken)
    {
        var offset = (request.PageNo - 1) * request.PageSize;

        var freelancerApplications = await _unitOfWork.FreelancerApplicationsRepository.GetAllPaginatedAsync(
            offset, request.PageSize, cancellationToken);

        var applicationsCount = await _unitOfWork.FreelancerApplicationsRepository.CountAllAsync(cancellationToken);

        return new PaginatedResultModel<FreelancerApplication>
        {
            Items = freelancerApplications.ToList(),
            TotalCount = applicationsCount,
            PageNo = request.PageNo,
            PageSize = request.PageSize
        };
    }
}