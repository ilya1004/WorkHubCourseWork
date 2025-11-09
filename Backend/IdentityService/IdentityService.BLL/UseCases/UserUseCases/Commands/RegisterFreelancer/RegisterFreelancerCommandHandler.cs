using IdentityService.BLL.Abstractions.EmailSender;
using IdentityService.DAL.Abstractions.RedisService;
using IdentityService.DAL.Constants;
using Microsoft.Extensions.Configuration;

namespace IdentityService.BLL.UseCases.UserUseCases.Commands.RegisterFreelancer;

public class RegisterFreelancerCommandHandler(
    UserManager<User> userManager,
    IUnitOfWork unitOfWork,
    IEmailSender emailSender,
    ICachedService cachedService,
    IConfiguration configuration,
    ILogger<RegisterFreelancerCommandHandler> logger) : IRequestHandler<RegisterFreelancerCommand>
{
    public async Task Handle(RegisterFreelancerCommand request, CancellationToken cancellationToken)
    {
        var userByEmail = await userManager.FindByEmailAsync(request.Email);

        if (userByEmail is not null)
        {
            logger.LogError("User with email {Email} already exists", request.Email);
            throw new AlreadyExistsException($"A user with the email '{request.Email}' already exists.");
        }

        var role = await unitOfWork.RolesRepository.GetByNameAsync(AppRoles.FreelancerRole, cancellationToken);

        if (role is null)
        {
            logger.LogError("Role is not found");
            throw new NotFoundException("Role is not found");
        }

        var passwordHash = request.Password; // TODO

        var user = new User
        {
            Id = Guid.NewGuid(),
            RegisteredAt = DateTime.UtcNow,
            Email = request.Email,
            PasswordHash = passwordHash,
            IsEmailConfirmed = false,
            IsActive = true,
            RoleId = role.Id,
        };

        var isSucceed = await unitOfWork.UsersRepository.CreateAsync(user, cancellationToken);

        if (isSucceed)
        {
            logger.LogError("User is not successfully created");
            throw new BadRequestException("User is not successfully created");
        }

        var freelancerProfile = new FreelancerProfile
        {
            Id = Guid.Empty,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Nickname = request.Nickname,
            UserId = user.Id,
        };

        await unitOfWork.FreelancerProfilesRepository.AddAsync(freelancerProfile, cancellationToken);
        await unitOfWork.SaveAllAsync(cancellationToken);

        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

        string code;
        var random = new Random();
        do
        {
            code = random.Next(100000, 999999).ToString();
        } 
        while (await cachedService.ExistsAsync(code, cancellationToken));

        await cachedService.SetAsync(
            code,
            token,
            TimeSpan.FromHours(int.Parse(
                configuration.GetRequiredSection("IdentityTokenExpirationTimeInHours").Value!)),
            cancellationToken);

        await emailSender.SendEmailConfirmation(user.Email, code, cancellationToken);
    }
}