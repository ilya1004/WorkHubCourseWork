using IdentityService.BLL.Abstractions.UserContext;
using IdentityService.BLL.DTOs;
using IdentityService.BLL.UseCases.UserUseCases.Queries.GetCurrentFreelancerUser;

namespace IdentityService.BLL.UseCases.UserUseCases.Queries.GetCurrentEmployerUser;

public class GetCurrentEmployerUserQueryHandler : IRequestHandler<GetCurrentEmployerUserQuery, EmployerUserDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    private readonly ILogger<GetCurrentFreelancerUserQueryHandler> _logger;

    public GetCurrentEmployerUserQueryHandler(
        IUnitOfWork unitOfWork,
        IUserContext userContext,
        ILogger<GetCurrentFreelancerUserQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
        _logger = logger;
    }

    public async Task<EmployerUserDto> Handle(GetCurrentEmployerUserQuery request, CancellationToken cancellationToken)
    {
        var userId = _userContext.GetUserId();

        var user = await _unitOfWork.UsersRepository.GetEmployerByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            _logger.LogError("User with ID '{UserId}' not found", userId);
            throw new NotFoundException($"User with ID '{userId}' not found");
        }

        var result = new EmployerUserDto
        {
            Id = user.Id,
            CompanyName = user.CompanyName,
            About = user.About,
            Email = user.Email,
            RegisteredAt = user.RegisteredAt,
            StripeCustomerId = user.StripeCustomerId,
            Industry = user.IndustryId is null
                ? null
                : new EmployerIndustryDto(
                    user.IndustryId.Value,
                    user.IndustryName!),
            ImageUrl = user.ImageUrl,
            RoleName = user.RoleName,
        };

        return result;
    }
}