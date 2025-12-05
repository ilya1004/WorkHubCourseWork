namespace ProjectsService.Domain.Abstractions.Data;

public interface IUnitOfWork
{
    ICategoriesRepository CategoriesRepository { get; }
    IFreelancerApplicationsRepository FreelancerApplicationsRepository { get; }
    ILifecyclesRepository LifecyclesRepository { get; }
    IProjectsRepository ProjectsRepository { get; }
    IReportsRepository ReportsRepository { get; }
    IStarredProjectsRepository StarredProjectsRepository { get; }
}