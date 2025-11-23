namespace IdentityService.BLL.UseCases.EmployerIndustryUseCases.Commands.UpdateEmployerIndustry;

public class UpdateEmployerIndustryCommandHandler : IRequestHandler<UpdateEmployerIndustryCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateEmployerIndustryCommandHandler> _logger;

    public UpdateEmployerIndustryCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<UpdateEmployerIndustryCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(UpdateEmployerIndustryCommand request, CancellationToken cancellationToken)
    {
        var industry = await _unitOfWork.EmployerIndustriesRepository.GetByIdAsync(request.Id, cancellationToken);

        if (industry is null)
        {
            _logger.LogError("Employer industry with ID {IndustryId} not found", request.Id);
            throw new NotFoundException($"Employer industry with ID '{request.Id}' not found");
        }

        industry.Name = request.Name;

        await _unitOfWork.EmployerIndustriesRepository.UpdateAsync(industry, cancellationToken);
    }
}