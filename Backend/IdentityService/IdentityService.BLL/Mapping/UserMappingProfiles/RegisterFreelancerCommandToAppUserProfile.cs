using IdentityService.BLL.UseCases.UserUseCases.Commands.RegisterFreelancer;

namespace IdentityService.BLL.Mapping.UserMappingProfiles;

public class RegisterFreelancerCommandToAppUserProfile : Profile
{
    public RegisterFreelancerCommandToAppUserProfile()
    {
        CreateMap<RegisterFreelancerCommand, User>()
            .ForMember(dest => dest.IsEmailConfirmed, opt =>
                opt.MapFrom(_ => false))
            .ForMember(dest => dest.RegisteredAt, opt =>
                opt.MapFrom(_ => DateTime.UtcNow));
    }
}