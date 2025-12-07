using ProjectsService.Application.Models;
using ProjectsService.Application.Specifications.FreelancerApplicationSpecifications;

namespace ProjectsService.Application.UseCases.Queries.FreelancerApplicationUseCases.GetFreelancerApplicationsByFilter;

public class GetFreelancerApplicationsByFilterQueryHandler : IRequestHandler<GetFreelancerApplicationsByFilterQuery,
    PaginatedResultModel<FreelancerApplication>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetFreelancerApplicationsByFilterQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PaginatedResultModel<FreelancerApplication>> Handle(
        GetFreelancerApplicationsByFilterQuery request,
        CancellationToken cancellationToken)
    {
        var offset = (request.PageNo - 1) * request.PageSize;

        var specification = new GetFreelancerApplicationsByFilterSpecification(
            request.StartDate,
            request.EndDate,
            request.ApplicationStatus,
            offset,
            request.PageSize);

        var applications = await _unitOfWork.FreelancerApplicationsRepository.GetByFilterAsync(
            request.StartDate, request.EndDate, request.ApplicationStatus, request.PageSize, offset, cancellationToken);

        var applicationsCount = await _unitOfWork.FreelancerApplicationsRepository.CountByFilterAsync(
            request.StartDate, request.EndDate, request.ApplicationStatus, request.PageSize, offset, cancellationToken);

        return new PaginatedResultModel<FreelancerApplication>
        {
            Items = applications.ToList(),
            TotalCount = applicationsCount,
            PageNo = request.PageNo,
            PageSize = request.PageSize
        };
    }
}