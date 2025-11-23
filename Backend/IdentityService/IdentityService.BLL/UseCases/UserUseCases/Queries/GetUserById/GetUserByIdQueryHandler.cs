using IdentityService.BLL.Abstractions.UserContext;
using IdentityService.DAL.Constants;

namespace IdentityService.BLL.UseCases.UserUseCases.Queries.GetUserById;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, User>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetUserByIdQueryHandler> _logger;
    private readonly IUserContext _userContext;

    public GetUserByIdQueryHandler(
        IUnitOfWork unitOfWork,
        ILogger<GetUserByIdQueryHandler> logger,
        IUserContext userContext)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _userContext = userContext;
    }

    public async Task<User> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var userRoleName = _userContext.GetRoleName();

        User user = null!;


        user = userRoleName switch
        {
            AppRoles.FreelancerRole => await _unitOfWork.UsersRepository.GetByIdAsync(
                request.Id, cancellationToken, true),
            AppRoles.EmployerRole => await _unitOfWork.UsersRepository.GetByIdAsync(
                request.Id, cancellationToken, true),
            AppRoles.AdminRole => await _unitOfWork.UsersRepository.GetByIdAsync(
                request.Id, cancellationToken),
            _ => null!
        };

        if (user is null)
        {
            _logger.LogError("User with ID '{UserId}' not found", request.Id);
            throw new NotFoundException($"User with ID '{request.Id}' not found");
        }
        
        return user;
    }
}