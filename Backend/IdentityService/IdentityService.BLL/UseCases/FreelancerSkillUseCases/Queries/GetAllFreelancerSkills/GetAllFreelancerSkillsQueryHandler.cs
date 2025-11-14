using IdentityService.BLL.Models;

namespace IdentityService.BLL.UseCases.FreelancerSkillUseCases.Queries.GetAllFreelancerSkills;

public class GetAllFreelancerSkillsQueryHandler(
    IUnitOfWork unitOfWork,
    ILogger<GetAllFreelancerSkillsQueryHandler> logger) 
    : IRequestHandler<GetAllFreelancerSkillsQuery, PaginatedResultModel<CvSkill>>
{
    public async Task<PaginatedResultModel<CvSkill>> Handle(GetAllFreelancerSkillsQuery request, 
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting paginated list of freelancer skills. Page: {PageNo}, Size: {PageSize}", 
            request.PageNo, request.PageSize);

        var offset = (request.PageNo - 1) * request.PageSize;

        var skills = await unitOfWork.CvSkillsRepository.PaginatedListAllAsync(
            offset, request.PageSize, cancellationToken);

        var skillsCount = await unitOfWork.CvSkillsRepository.CountAllAsync(cancellationToken);

        logger.LogInformation("Retrieved {Count} skills out of {TotalCount}", skills.Count, skillsCount);

        return new PaginatedResultModel<CvSkill>
        {
            Items = skills.ToList(),
            TotalCount = skillsCount,
            PageNo = request.PageNo,
            PageSize = request.PageSize
        };
    }
}