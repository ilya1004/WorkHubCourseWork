namespace ProjectsService.Application.UseCases.Commands.CategoryUseCases.UpdateCategory;

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateCategoryCommandHandler> _logger;

    public UpdateCategoryCommandHandler(IUnitOfWork unitOfWork,
        ILogger<UpdateCategoryCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _unitOfWork.CategoriesRepository.GetByIdAsync(request.Id, cancellationToken);

        if (category is null)
        {
            _logger.LogError("Category with ID '{CategoryId}' not found", request.Id);
            throw new NotFoundException($"Category with ID '{request.Id}' not found");
        }

        category.Name = request.Name;

        await _unitOfWork.CategoriesRepository.UpdateAsync(category, cancellationToken);
    }
}