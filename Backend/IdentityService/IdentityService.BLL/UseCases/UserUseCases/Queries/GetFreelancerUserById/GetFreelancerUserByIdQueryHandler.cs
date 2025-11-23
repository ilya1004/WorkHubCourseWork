using IdentityService.BLL.DTOs;
using IdentityService.BLL.UseCases.UserUseCases.Queries.GetFreelancerUserById;

namespace IdentityService.BLL.UseCases.UserUseCases.Queries.GetFreelancerUserInfoById;

public class GetFreelancerUserByIdQueryHandler : IRequestHandler<GetFreelancerUserByIdQuery, FreelancerUserDto>
{
    private readonly ILogger<GetFreelancerUserByIdQueryHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public GetFreelancerUserByIdQueryHandler(ILogger<GetFreelancerUserByIdQueryHandler> logger,
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<FreelancerUserDto> Handle(GetFreelancerUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.UsersRepository.GetFreelancerByIdAsync(request.Id, cancellationToken);

        if (user is null)
        {
            _logger.LogError("User with ID '{UserId}' not found", request.Id);
            throw new NotFoundException($"User with ID '{request.Id}' not found");
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