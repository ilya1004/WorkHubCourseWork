namespace ProjectsService.Application.UseCases.Queries.CategoryUseCases.GetCategoryById;

public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, Category>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetCategoryByIdQueryHandler> _logger;

    public GetCategoryByIdQueryHandler(
        IUnitOfWork unitOfWork,
        ILogger<GetCategoryByIdQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Category> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await _unitOfWork.CategoriesRepository.GetByIdAsync(request.Id, cancellationToken);

        if (category is null)
        {
            _logger.LogError("Category with ID {CategoryId} not found", request.Id);
            throw new NotFoundException($"Category with ID '{request.Id}' not found");
        }

        return category;
    }
}