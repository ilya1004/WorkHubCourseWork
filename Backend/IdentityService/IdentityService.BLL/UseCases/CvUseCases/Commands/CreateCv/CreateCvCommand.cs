using IdentityService.BLL.DTOs;

namespace IdentityService.BLL.UseCases.CvUseCases.Commands.CreateCv;

public sealed record CreateCvCommand(
    string Title,
    string UserSpecialization,
    string? UserEducation,
    bool IsPublic,
    List<CvLanguageDto> Languages,
    List<CvSkillDto> Skills,
    List<CvWorkExperienceDto> WorkExperiences) : IRequest<Cv>;