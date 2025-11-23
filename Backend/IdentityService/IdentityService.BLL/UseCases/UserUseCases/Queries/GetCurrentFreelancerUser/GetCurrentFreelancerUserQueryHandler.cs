using IdentityService.BLL.Abstractions.UserContext;
using IdentityService.BLL.DTOs;

namespace IdentityService.BLL.UseCases.UserUseCases.Queries.GetCurrentFreelancerUser;

public class GetCurrentFreelancerUserQueryHandler : IRequestHandler<GetCurrentFreelancerUserQuery, FreelancerUserDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    private readonly ILogger<GetCurrentFreelancerUserQueryHandler> _logger;

    public GetCurrentFreelancerUserQueryHandler(IUnitOfWork unitOfWork,
        IUserContext userContext,
        ILogger<GetCurrentFreelancerUserQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
        _logger = logger;
    }

    public async Task<FreelancerUserDto> Handle(GetCurrentFreelancerUserQuery request, CancellationToken cancellationToken)
    {
        var userId = _userContext.GetUserId();

        var user = await _unitOfWork.UsersRepository.GetFreelancerByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            _logger.LogError("User with ID '{UserId}' not found", userId);
            throw new NotFoundException($"User with ID '{userId}' not found");
        }

        var result = new FreelancerUserDto
        {
            Id = user.Id,
            Nickname = user.Nickname,
            FirstName = user.FirstName,
            LastName = user.LastName,
            About = user.About,
            Email = user.Email,
            RegisteredAt = user.RegisteredAt,
            StripeAccountId = user.StripeAccountId,
            ImageUrl = user.ImageUrl,
            RoleName = user.RoleName,
        };

        return result;
    }
}