namespace ProjectsService.Application.Specifications.ProjectSpecifications;

public class GetProjectsByFreelancerFilterSpecification : Specification<Project>
{
    public GetProjectsByFreelancerFilterSpecification(
        Guid freelancerId,
        ProjectStatus? projectStatus,
        Guid? employerId,
        int offset,
        int limit)
        : base(p => 
            p.FreelancerUserId == freelancerId &&
            (!projectStatus.HasValue || p.Lifecycle.ProjectStatus == projectStatus.Value) &&
            (!employerId.HasValue || p.EmployerUserId == employerId))
    {
        AddInclude(p => p.Lifecycle);
        AddInclude(p => p.Category!);
        AddOrderByDescending(p => p.Lifecycle.CreatedAt);
        AddPagination(offset, limit);
    }
}