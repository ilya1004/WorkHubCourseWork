namespace IdentityService.BLL.UseCases.CvUseCases.Commands.DeleteCv;

public class DeleteCvCommandHandler : IRequestHandler<DeleteCvCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteCvCommandHandler> _logger;

    public DeleteCvCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteCvCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(DeleteCvCommand request, CancellationToken cancellationToken)
    {
        var cv = await _unitOfWork.CvsRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (cv is null)
        {
            _logger.LogError("CV with ID {CvId} not found", request.Id);
            throw new NotFoundException($"CV with ID '{request.Id}' not found");
        }

        await _unitOfWork.CvLanguagesRepository.DeleteByCvIdAsync(request.Id, cancellationToken);
        await _unitOfWork.CvSkillsRepository.DeleteByCvIdAsync(request.Id, cancellationToken);
        await _unitOfWork.CvWorkExperiencesRepository.DeleteByCvIdAsync(request.Id, cancellationToken);

        await _unitOfWork.CvsRepository.DeleteAsync(request.Id, cancellationToken);
    }
}