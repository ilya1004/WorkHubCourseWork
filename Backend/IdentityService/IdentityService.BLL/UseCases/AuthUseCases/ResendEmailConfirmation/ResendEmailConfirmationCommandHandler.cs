using IdentityService.BLL.Abstractions.EmailSender;
using IdentityService.DAL.Abstractions.RedisService;
using Microsoft.Extensions.Configuration;

namespace IdentityService.BLL.UseCases.AuthUseCases.ResendEmailConfirmation;

public class ResendEmailConfirmationCommandHandler(
    UserManager<User> userManager, 
    IEmailSender emailSender,
    ICachedService cachedService,
    IConfiguration configuration,
    ILogger<ResendEmailConfirmationCommandHandler> logger) : IRequestHandler<ResendEmailConfirmationCommand>
{
    public async Task Handle(ResendEmailConfirmationCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            logger.LogError("User with email {Email} not found", request.Email);
            throw new BadRequestException($"A user with the email '{request.Email}' not exist.");
        }

        if (user.IsEmailConfirmed)
        {
            logger.LogError("Email {Email} already confirmed", request.Email);
            throw new BadRequestException("Your email is already confirmed.");
        }

        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

        string code;
        var random = new Random();
        do
        {
            code = random.Next(100000, 999999).ToString();
        }
        while (await cachedService.ExistsAsync(code, cancellationToken));

        await cachedService.SetAsync(code, token, TimeSpan.FromHours(
            int.Parse(configuration.GetRequiredSection("IdentityTokenExpirationTimeInHours").Value!)), cancellationToken);

        await emailSender.SendEmailConfirmation(user.Email!, code, cancellationToken);
    }
}