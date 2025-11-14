namespace IdentityService.BLL.UseCases.EmployerIndustryUseCases.Commands.CreateEmployerIndustry;

public class CreateEmployerIndustryCommandHandler : IRequestHandler<CreateEmployerIndustryCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateEmployerIndustryCommandHandler> _logger;

    public CreateEmployerIndustryCommandHandler(IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<CreateEmployerIndustryCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task Handle(CreateEmployerIndustryCommand request, CancellationToken cancellationToken)
    {
        var industry = await _unitOfWork.EmployerIndustriesRepository.GetByNameAsync(request.Name, cancellationToken);

        if (industry != null)
        {
            _logger.LogError("Industry with name {IndustryName} already exists", request.Name);
            throw new BadRequestException($"Industry with the name '{request.Name}' already exists.");
        }

        var newIndustry = new EmployerIndustry
        {
            Id = Guid.CreateVersion7(),
            Name = request.Name,
        };

        await _unitOfWork.EmployerIndustriesRepository.CreateAsync(newIndustry, cancellationToken);
    }
}