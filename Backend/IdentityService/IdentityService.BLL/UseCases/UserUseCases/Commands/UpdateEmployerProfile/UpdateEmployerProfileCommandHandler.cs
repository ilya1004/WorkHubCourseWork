using IdentityService.BLL.Abstractions.BlobService;
using IdentityService.BLL.Abstractions.UserContext;

namespace IdentityService.BLL.UseCases.UserUseCases.Commands.UpdateEmployerProfile;

public class UpdateEmployerProfileCommandHandler : IRequestHandler<UpdateEmployerProfileCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBlobService _blobService;
    private readonly IUserContext _userContext;
    private readonly ILogger<UpdateEmployerProfileCommandHandler> _logger;

    public UpdateEmployerProfileCommandHandler(
        IUnitOfWork unitOfWork,
        IBlobService blobService,
        IUserContext userContext,
        ILogger<UpdateEmployerProfileCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _blobService = blobService;
        _userContext = userContext;
        _logger = logger;
    }

    public async Task Handle(UpdateEmployerProfileCommand request, CancellationToken cancellationToken)
    {
        var userId = _userContext.GetUserId();

        var user = await _unitOfWork.UsersRepository.GetByIdAsync(
            userId,
            cancellationToken,
            true);

        if (user?.EmployerProfile is null)
        {
            _logger.LogError("User with ID '{UserId}' not found", userId);
            throw new NotFoundException($"User with ID '{userId}' not found");
        }

        user.EmployerProfile.CompanyName = request.EmployerProfile.CompanyName;
        user.EmployerProfile.About = request.EmployerProfile.About;

        if (request.EmployerProfile.IndustryId.HasValue)
        {
            if (request.EmployerProfile.IndustryId.Value != user.EmployerProfile.IndustryId)
            {
                var industry = await _unitOfWork.EmployerIndustriesRepository.GetByIdAsync(
                    request.EmployerProfile.IndustryId.Value, cancellationToken);

                if (industry is null)
                {
                    _logger.LogError("Industry with ID {IndustryId} not found", request.EmployerProfile.IndustryId);
                    throw new NotFoundException($"Industry with ID '{request.EmployerProfile.IndustryId}' not found");
                }

                user.EmployerProfile.IndustryId = industry.Id;
            }
        }
        else if (user.EmployerProfile.IndustryId is not null)
        {
            user.EmployerProfile.IndustryId = null;
        }
        
        if (request.ResetImage)
        {
            user.ImageUrl = null;
        }

        if (request.FileStream is not null && request.ContentType is not null)
        {
            if (!request.ContentType.StartsWith("image/"))
            {
                _logger.LogError("Invalid file type: {ContentType}", request.ContentType);
                throw new BadRequestException("Only image files are allowed.");
            }
            
            if (!string.IsNullOrEmpty(user.ImageUrl) && Guid.TryParse(user.ImageUrl, out var imageId))
            {
                await _blobService.DeleteAsync(imageId, cancellationToken);
            }

            var imageFileId = await _blobService.UploadAsync(
                request.FileStream,
                request.ContentType!,
                cancellationToken);

            user.ImageUrl = imageFileId.ToString();
        }

        await _unitOfWork.UsersRepository.UpdateUserImageAsync(user.Id, user.ImageUrl, cancellationToken);
        await _unitOfWork.EmployerProfilesRepository.UpdateAsync(
            user.EmployerProfile.Id,
            user.EmployerProfile,
            cancellationToken);
    }
}