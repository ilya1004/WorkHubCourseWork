using IdentityService.DAL.Abstractions.RedisService;

namespace IdentityService.BLL.UseCases.AuthUseCases.ConfirmEmail;

public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand>
{
    private readonly ICachedService _cachedService;
    private readonly ILogger<ConfirmEmailCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public ConfirmEmailCommandHandler(
        ICachedService cachedService,
        ILogger<ConfirmEmailCommandHandler> logger,
        IUnitOfWork unitOfWork)
    {
        _cachedService = cachedService;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.UsersRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user is null)
        {
            _logger.LogError("User with email {Email} not found", request.Email);
            throw new BadRequestException($"A user with the email '{request.Email}' not exist.");
        }

        if (user.IsEmailConfirmed)
        {
            _logger.LogError("Email {Email} already confirmed", request.Email);
            throw new BadRequestException("Your email is already confirmed.");
        }
        
        var token = await _cachedService.GetAsync(request.Code, cancellationToken);

        if (string.IsNullOrEmpty(token))
        {
            _logger.LogError("Invalid verification code {Code}", request.Code);
            throw new BadRequestException("Invalid verification code.");
        }

        await _unitOfWork.UsersRepository.UpdateIsEmailConfirmedAsync(user.Id, cancellationToken);

        await _cachedService.DeleteAsync(request.Code, cancellationToken);
    }
}