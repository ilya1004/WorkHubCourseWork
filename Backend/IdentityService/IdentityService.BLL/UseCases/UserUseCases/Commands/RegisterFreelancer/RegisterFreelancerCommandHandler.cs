using IdentityService.BLL.Abstractions.EmailSender;
using IdentityService.BLL.Abstractions.TokenProvider;
using IdentityService.DAL.Abstractions.PasswordHasher;
using IdentityService.DAL.Abstractions.RedisService;
using IdentityService.DAL.Constants;
using Microsoft.Extensions.Configuration;

namespace IdentityService.BLL.UseCases.UserUseCases.Commands.RegisterFreelancer;

public class RegisterFreelancerCommandHandler : IRequestHandler<RegisterFreelancerCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailSender _emailSender;
    private readonly ICachedService _cachedService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<RegisterFreelancerCommandHandler> _logger;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenProvider _tokenProvider;

    public RegisterFreelancerCommandHandler(
        IUnitOfWork unitOfWork,
        IEmailSender emailSender,
        ICachedService cachedService,
        IConfiguration configuration,
        ILogger<RegisterFreelancerCommandHandler> logger,
        IPasswordHasher passwordHasher,
        ITokenProvider tokenProvider)
    {
        _unitOfWork = unitOfWork;
        _emailSender = emailSender;
        _cachedService = cachedService;
        _configuration = configuration;
        _logger = logger;
        _passwordHasher = passwordHasher;
        _tokenProvider = tokenProvider;
    }

    public async Task Handle(RegisterFreelancerCommand request, CancellationToken cancellationToken)
    {
        var userByEmail = await _unitOfWork.UsersRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (userByEmail is not null)
        {
            _logger.LogError("User with email {Email} already exists", request.Email);
            throw new AlreadyExistsException($"A user with the email '{request.Email}' already exists.");
        }

        var passwordHash = _passwordHasher.HashPassword(request.Password);

        var role = await _unitOfWork.RolesRepository.GetByNameAsync(AppRoles.FreelancerRole, cancellationToken);

        if (role is null)
        {
            _logger.LogError("Freelancer role not found");
            throw new BadRequestException("User is not successfully registered. User Role is not successfully find");
        }

        var user = new User
        {
            Id = Guid.CreateVersion7(),
            RegisteredAt = DateTime.UtcNow,
            Email = request.Email,
            PasswordHash = passwordHash,
            IsEmailConfirmed = false,
            RoleId = role.Id,
        };

        await _unitOfWork.UsersRepository.CreateAsync(user, cancellationToken);

        var freelancerProfile = new FreelancerProfile
        {
            Id = Guid.CreateVersion7(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Nickname = request.Nickname,
            UserId = user.Id,
        };

        await _unitOfWork.FreelancerProfilesRepository.CreateAsync(freelancerProfile, cancellationToken);

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

        await _emailSender.SendEmailConfirmation(user.Email, code, cancellationToken);
    }
}