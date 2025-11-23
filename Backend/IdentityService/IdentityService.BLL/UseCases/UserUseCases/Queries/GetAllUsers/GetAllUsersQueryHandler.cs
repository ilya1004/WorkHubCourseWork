using IdentityService.BLL.Models;

namespace IdentityService.BLL.UseCases.UserUseCases.Queries.GetAllUsers;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, PaginatedResultModel<User>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllUsersQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PaginatedResultModel<User>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var offset = (request.PageNo - 1) * request.PageSize;

        var users = await _unitOfWork.UsersRepository.GetAllPaginatedAsync(
            offset,
            request.PageSize,
            cancellationToken);
        
        var usersCount = await _unitOfWork.UsersRepository.CountAllAsync(cancellationToken);

        return new PaginatedResultModel<User>
        {
            Items = users.ToList(),
            TotalCount = usersCount,
            PageNo = request.PageNo,
            PageSize = request.PageSize
        };
    }
}