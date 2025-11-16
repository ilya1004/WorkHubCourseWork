namespace IdentityService.BLL.UseCases.EmployerIndustryUseCases.Commands.DeleteEmployerIndustry;

public class DeleteEmployerIndustryCommandHandler : IRequestHandler<DeleteEmployerIndustryCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteEmployerIndustryCommandHandler> _logger;

    public DeleteEmployerIndustryCommandHandler(IUnitOfWork unitOfWork,
        ILogger<DeleteEmployerIndustryCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(DeleteEmployerIndustryCommand request, CancellationToken cancellationToken)
    {
        var industry = await _unitOfWork.EmployerIndustriesRepository.GetByIdAsync(request.Id, cancellationToken);

        if (industry is null)
        {
            _logger.LogError("Employer industry with ID {IndustryId} not found", request.Id);
            throw new NotFoundException($"Employer Industry with ID '{request.Id}' not found");
        }

        await _unitOfWork.EmployerIndustriesRepository.DeleteAsync(industry.Id, cancellationToken);
    }
}