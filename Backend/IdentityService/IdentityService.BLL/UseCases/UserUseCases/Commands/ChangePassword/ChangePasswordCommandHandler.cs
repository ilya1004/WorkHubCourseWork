namespace IdentityService.BLL.UseCases.UserUseCases.Commands.ChangePassword;

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand>
{
    private readonly ILogger<ChangePasswordCommandHandler> _logger;

    public ChangePasswordCommandHandler(UserManager<User> userManager,
        ILogger<ChangePasswordCommandHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Changing password for user with email: {Email}", request.Email);

        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            _logger.LogWarning("User with email {Email} not found", request.Email);
            
            throw new NotFoundException($"User with email '{request.Email}' not found");
        }

        var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            
            _logger.LogWarning("Failed to change password for user {UserId}: {Errors}", user.Id, errors);
            
            throw new BadRequestException($"Password is not successfully changed. Errors: {errors}");
        }

        _logger.LogInformation("Successfully changed password for user {UserId}", user.Id);
    }
}