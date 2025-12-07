using ProjectsService.Application.Models;
using ProjectsService.Application.Specifications.FreelancerApplicationSpecifications;
using ProjectsService.Domain.Abstractions.UserContext;

namespace ProjectsService.Application.UseCases.Queries.FreelancerApplicationUseCases.GetMyFreelancerApplicationsByFilter;

public class GetMyFreelancerApplicationsByFilterQueryHandler : IRequestHandler<GetMyFreelancerApplicationsByFilterQuery,
    PaginatedResultModel<FreelancerApplication>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;

    public GetMyFreelancerApplicationsByFilterQueryHandler(
        IUnitOfWork unitOfWork,
        IUserContext userContext)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
    }

    public async Task<PaginatedResultModel<FreelancerApplication>> Handle(
        GetMyFreelancerApplicationsByFilterQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _userContext.GetUserId();

        var offset = (request.PageNo - 1) * request.PageSize;

        var specification = new GetMyFreelancerApplicationsByFilterSpecification(
            userId,
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