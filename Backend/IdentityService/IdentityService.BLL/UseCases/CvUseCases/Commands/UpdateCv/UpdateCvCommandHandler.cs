namespace IdentityService.BLL.UseCases.CvUseCases.Commands.UpdateCv;

public class UpdateCvCommandHandler : IRequestHandler<UpdateCvCommand, Cv>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateCvCommandHandler> _logger;

    public UpdateCvCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateCvCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Cv> Handle(UpdateCvCommand request, CancellationToken cancellationToken)
    {
        var existingCv = await _unitOfWork.CvsRepository.GetByIdAsync(request.Id, cancellationToken);

        if (existingCv is null)
        {
            _logger.LogError("CV with ID {CvId} not found", request.Id);
            throw new NotFoundException($"CV with ID '{request.Id}' not found");
        }

        existingCv.Title = request.Title;
        existingCv.UserSpecialization = request.UserSpecialization;
        existingCv.UserEducation = request.UserEducation;
        existingCv.IsPublic = request.IsPublic;

        await _unitOfWork.CvsRepository.UpdateAsync(existingCv, cancellationToken);

        await _unitOfWork.CvLanguagesRepository.DeleteByCvIdAsync(request.Id, cancellationToken);
        await _unitOfWork.CvSkillsRepository.DeleteByCvIdAsync(request.Id, cancellationToken);
        await _unitOfWork.CvWorkExperiencesRepository.DeleteByCvIdAsync(request.Id, cancellationToken);

        foreach (var langDto in request.Languages)
        {
            var lang = new CvLanguage
            {
                Id = Guid.CreateVersion7(),
                Name = langDto.Name,
                Level = langDto.Level,
                CvId = request.Id
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
                CvId = request.Id
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
                CvId = request.Id
            };
            await _unitOfWork.CvWorkExperiencesRepository.AddAsync(exp, cancellationToken);
        }

        var cvEntity = await _unitOfWork.CvsRepository.GetByIdAsync(existingCv.Id, cancellationToken, true);

        if (cvEntity is null)
        {
            _logger.LogError("Cv with ID {CvId} not found", existingCv.Id);
            throw new NotFoundException($"Cv with ID '{existingCv.Id}' not found");
        }

        return cvEntity;
    }
}