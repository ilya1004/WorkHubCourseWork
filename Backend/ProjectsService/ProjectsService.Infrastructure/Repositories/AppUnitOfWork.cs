using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using ProjectsService.Infrastructure.Data;

namespace ProjectsService.Infrastructure.Repositories;

public class AppUnitOfWork : IUnitOfWork
{
    public AppUnitOfWork(ICategoriesRepository categoriesRepository, IFreelancerApplicationsRepository freelancerApplicationsRepository, ILifecyclesRepository lifecyclesRepository, IProjectsRepository projectsRepository, IReportsRepository reportsRepository, IStarredProjectsRepository starredProjectsRepository)
    {
        CategoriesRepository = categoriesRepository;
        FreelancerApplicationsRepository = freelancerApplicationsRepository;
        LifecyclesRepository = lifecyclesRepository;
        ProjectsRepository = projectsRepository;
        ReportsRepository = reportsRepository;
        StarredProjectsRepository = starredProjectsRepository;
    }

    public ICategoriesRepository CategoriesRepository { get; }
    public IFreelancerApplicationsRepository FreelancerApplicationsRepository { get; }
    public ILifecyclesRepository LifecyclesRepository { get; }
    public IProjectsRepository ProjectsRepository { get; }
    public IReportsRepository ReportsRepository { get; }
    public IStarredProjectsRepository StarredProjectsRepository { get; }
}
