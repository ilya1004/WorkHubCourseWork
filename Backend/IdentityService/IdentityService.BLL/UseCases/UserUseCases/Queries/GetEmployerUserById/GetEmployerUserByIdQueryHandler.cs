using IdentityService.BLL.DTOs;

namespace IdentityService.BLL.UseCases.UserUseCases.Queries.GetEmployerUserById;

public class GetEmployerUserByIdQueryHandler : IRequestHandler<GetEmployerUserByIdQuery, EmployerUserDto>
{
    private readonly ILogger<GetEmployerUserByIdQueryHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public GetEmployerUserByIdQueryHandler(ILogger<GetEmployerUserByIdQueryHandler> logger,
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<EmployerUserDto> Handle(GetEmployerUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.UsersRepository.GetEmployerByIdAsync(request.Id, cancellationToken);
        
        if (user is null)
        {
            _logger.LogError("User with ID '{UserId}' not found", request.Id);
            throw new NotFoundException($"User with ID '{request.Id}' not found");
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