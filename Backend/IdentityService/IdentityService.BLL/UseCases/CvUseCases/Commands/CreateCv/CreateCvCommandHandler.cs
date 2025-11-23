using IdentityService.BLL.Abstractions.UserContext;
using IdentityService.BLL.DTOs;

namespace IdentityService.BLL.UseCases.CvUseCases.Commands.CreateCv;

public class CreateCvCommandHandler : IRequestHandler<CreateCvCommand, Cv>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    private readonly ILogger<CreateCvCommandHandler> _logger;

    public CreateCvCommandHandler(
        IUnitOfWork unitOfWork,
        IUserContext userContext,
        ILogger<CreateCvCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
        _logger = logger;
    }

    public async Task<Cv> Handle(CreateCvCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _userContext.GetUserId();

        var cvId = Guid.CreateVersion7();

        var cv = new Cv
        {
            Id = cvId,
            Title = request.Title,
            UserSpecialization = request.UserSpecialization,
            UserEducation = request.UserEducation,
            IsPublic = request.IsPublic,
            FreelancerUserId = currentUserId
        };

        await _unitOfWork.CvsRepository.CreateAsync(cv, cancellationToken);

        foreach (var langDto in request.Languages)
        {
            var lang = new CvLanguage
            {
                Id = Guid.CreateVersion7(),
                Name = langDto.Name,
                Level = langDto.Level,
                CvId = cvId
            };
            await _unitOfWork.CvLanguagesRepository.AddAsync(lang, cancellationToken);
        }

        foreach (var skillDto in request.Skills)
        {
            var skill = new CvSkill
            {
                Id = Guid.CreateVersion7(),
                Name = skillDto.Name,
                ExperienceInYears = skillDto.ExperienceInYears,
                CvId = cvId
            };
            await _unitOfWork.CvSkillsRepository.AddAsync(skill, cancellationToken);
        }

        foreach (var expDto in request.WorkExperiences)
        {
            var exp = new CvWorkExperience
            {
                Id = Guid.CreateVersion7(),
                UserSpecialization = expDto.UserSpecialization,
                StartDate = DateOnly.FromDateTime(expDto.StartDate),
                EndDate = DateOnly.FromDateTime(expDto.EndDate),
                Responsibilities = expDto.Responsibilities,
                CvId = cvId
            };
            await _unitOfWork.CvWorkExperiencesRepository.AddAsync(exp, cancellationToken);
        }

        var cvEntity = await _unitOfWork.CvsRepository.GetByIdAsync(cvId, cancellationToken, true);

        if (cvEntity is null)
        {
            _logger.LogError("Cv with ID {CvId} not found", cvId);
            throw new NotFoundException($"Cv with ID '{cvId}' not found");
        }

        return cvEntity;
    }
}