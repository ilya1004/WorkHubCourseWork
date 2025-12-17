using AutoMapper;
using ProjectsService.Domain.Enums;
using ProjectsService.Domain.Models;

namespace ProjectsService.Infrastructure.MappingProfiles;

public class ProjectInfoProfile : Profile
{
    public ProjectInfoProfile()
    {
        CreateMap<ProjectInfoView, ProjectInfo>()
            .ForMember(x => x.ProjectStatus, options =>
                options.MapFrom(x => Enum.Parse<ProjectStatus>(x.ProjectStatus)))
            .ForMember(x => x.AcceptanceStatus, options =>
                options.MapFrom(x => Enum.Parse<ProjectAcceptanceStatus>(x.AcceptanceStatus)));
    }
}