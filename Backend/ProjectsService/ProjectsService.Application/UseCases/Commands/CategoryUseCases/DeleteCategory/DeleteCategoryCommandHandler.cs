namespace ProjectsService.Application.UseCases.Commands.CategoryUseCases.DeleteCategory;

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteCategoryCommandHandler> _logger;

    public DeleteCategoryCommandHandler(IUnitOfWork unitOfWork,
        ILogger<DeleteCategoryCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _unitOfWork.CategoriesRepository.GetByIdAsync(request.Id, cancellationToken);

        if (category is null)
        {
            _logger.LogError("Category with ID '{CategoryId}' not found", request.Id);
            throw new NotFoundException($"Category with ID '{request.Id}' not found");
        }

        await _unitOfWork.CategoriesRepository.DeleteAsync(category.Id, cancellationToken);
    }
}