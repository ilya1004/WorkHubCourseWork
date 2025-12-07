using ProjectsService.Application.Models;

namespace ProjectsService.Application.UseCases.Queries.CategoryUseCases.GetAllCategories;

public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, PaginatedResultModel<Category>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllCategoriesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PaginatedResultModel<Category>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        var offset = (request.PageNo - 1) * request.PageSize;

        var categories = await _unitOfWork.CategoriesRepository.GetAllPaginatedAsync(
            offset,
            request.PageSize,
            cancellationToken);
        
        var categoriesCount = await _unitOfWork.CategoriesRepository.CountAllAsync(cancellationToken);

        return new PaginatedResultModel<Category>
        {
            Items = categories.ToList(),
            TotalCount = categoriesCount,
            PageNo = request.PageNo,
            PageSize = request.PageSize
        };
    }
}