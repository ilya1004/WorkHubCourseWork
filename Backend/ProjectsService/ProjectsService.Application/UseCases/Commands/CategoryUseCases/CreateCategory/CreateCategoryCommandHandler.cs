namespace ProjectsService.Application.UseCases.Commands.CategoryUseCases.CreateCategory;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateCategoryCommandHandler> _logger;

    public CreateCategoryCommandHandler(IUnitOfWork unitOfWork,
        ILogger<CreateCategoryCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _unitOfWork.CategoriesRepository.GetByName(request.Name, cancellationToken);

        if (category is not null)
        {
            _logger.LogError("Category with name '{CategoryName}' already exists", request.Name);
            throw new AlreadyExistsException($"Category with name '{request.Name}' already exists.");
        }

        var newCategory = new Category
        {
            Id = Guid.CreateVersion7(),
            Name = request.Name,
        };

        await _unitOfWork.CategoriesRepository.CreateAsync(newCategory, cancellationToken);
    }
}