using IdentityService.BLL.Abstractions.EmailSender;
using IdentityService.BLL.Abstractions.TokenProvider;
using IdentityService.DAL.Abstractions.RedisService;
using Microsoft.Extensions.Configuration;

namespace IdentityService.BLL.UseCases.AuthUseCases.ForgotPassword;

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand>
{
    private readonly IEmailSender _emailSender;
    private readonly ICachedService _cachedService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ForgotPasswordCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenProvider _tokenProvider;

    public ForgotPasswordCommandHandler(
        IEmailSender emailSender,
        ICachedService cachedService,
        IConfiguration configuration,
        ILogger<ForgotPasswordCommandHandler> logger,
        IUnitOfWork unitOfWork,
        ITokenProvider tokenProvider)
    {
        _emailSender = emailSender;
        _cachedService = cachedService;
        _configuration = configuration;
        _logger = logger;
        _unitOfWork = unitOfWork;
        _tokenProvider = tokenProvider;
    }

    public async Task Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.UsersRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user is null)
        {
            _logger.LogError("User with email {Email} not found", request.Email);
            throw new NotFoundException("User with this email does not exist.");
        }

        var token = _tokenProvider.GeneratePasswordResetToken(user);

        string code;
        var random = new Random();
        do
        {
            code = random.Next(100000, 999999).ToString();
        } 
        while (await _cachedService.ExistsAsync(code, cancellationToken));

        await _cachedService.SetAsync(code, token, TimeSpan.FromHours(
            int.Parse(_configuration.GetRequiredSection("IdentityTokenExpirationTimeInHours").Value!)), cancellationToken);

        var resetUrl = $"{request.ResetUrl}?email={user.Email}&code={code}";
        
        await _emailSender.SendPasswordReset(user.Email!, resetUrl, cancellationToken);
    }
}