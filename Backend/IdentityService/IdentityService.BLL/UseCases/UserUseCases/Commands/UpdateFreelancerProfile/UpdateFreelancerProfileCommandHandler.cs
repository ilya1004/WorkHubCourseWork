using IdentityService.BLL.Abstractions.BlobService;
using IdentityService.BLL.Abstractions.UserContext;

namespace IdentityService.BLL.UseCases.UserUseCases.Commands.UpdateFreelancerProfile;

public class UpdateFreelancerProfileCommandHandler : IRequestHandler<UpdateFreelancerProfileCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IBlobService _blobService;
    private readonly IUserContext _userContext;
    private readonly ILogger<UpdateFreelancerProfileCommandHandler> _logger;

    public UpdateFreelancerProfileCommandHandler(IUnitOfWork unitOfWork,
        IMapper mapper,
        IBlobService blobService,
        IUserContext userContext,
        ILogger<UpdateFreelancerProfileCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _blobService = blobService;
        _userContext = userContext;
        _logger = logger;
    }

    public async Task Handle(UpdateFreelancerProfileCommand request, CancellationToken cancellationToken)
    {
        var userId = _userContext.GetUserId();

        var user = await _unitOfWork.UsersRepository.GetByIdAsync(
            userId,
            cancellationToken,
            true);

        if (user?.FreelancerProfile is null)
        {
            _logger.LogError("User with ID '{userId}' not found", userId.ToString());
            throw new NotFoundException($"User with ID '{userId}' not found");
        }

        user.FreelancerProfile.FirstName = request.FreelancerProfile.FirstName;
        user.FreelancerProfile.LastName = request.FreelancerProfile.LastName;
        user.FreelancerProfile.Nickname = request.FreelancerProfile.Nickname;
        user.FreelancerProfile.About = request.FreelancerProfile.About;

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
                request.ContentType,
                cancellationToken);

            user.ImageUrl = imageFileId.ToString();
        }
        
        await _unitOfWork.UsersRepository.UpdateUserImageAsync(user.Id, user.ImageUrl, cancellationToken);
        await _unitOfWork.FreelancerProfilesRepository.UpdateAsync(
            user.FreelancerProfile.Id,
            user.FreelancerProfile,
            cancellationToken);
    }
}