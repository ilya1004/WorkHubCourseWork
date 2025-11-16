namespace IdentityService.BLL.UseCases.EmployerIndustryUseCases.Queries.GetEmployerIndustryById;

public class GetEmployerIndustryByIdQueryHandler : IRequestHandler<GetEmployerIndustryByIdQuery, EmployerIndustry>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetEmployerIndustryByIdQueryHandler> _logger;

    public GetEmployerIndustryByIdQueryHandler(IUnitOfWork unitOfWork,
        ILogger<GetEmployerIndustryByIdQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<EmployerIndustry> Handle(GetEmployerIndustryByIdQuery request, CancellationToken cancellationToken)
    {
        var industry = await _unitOfWork.EmployerIndustriesRepository.GetByIdAsync(request.Id, cancellationToken);

        if (industry is null)
        {
            _logger.LogError("Employer industry with ID {IndustryId} not found", request.Id);
            throw new NotFoundException($"Employer Industry with ID '{request.Id}' not found");
        }

        return industry;
    }
}