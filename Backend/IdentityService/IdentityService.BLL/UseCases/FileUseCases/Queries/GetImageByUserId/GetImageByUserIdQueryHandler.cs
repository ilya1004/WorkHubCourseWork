using IdentityService.BLL.Abstractions.BlobService;
using IdentityService.BLL.DTOs;

namespace IdentityService.BLL.UseCases.FileUseCases.Queries.GetImageByUserId;

public class GetImageByUserIdQueryHandler : IRequestHandler<GetImageByUserIdQuery, FileResponseDto>
{
    private readonly IBlobService _blobService;
    private readonly ILogger<GetImageByUserIdQueryHandler> _logger;

    public GetImageByUserIdQueryHandler(
        IBlobService blobService,
        ILogger<GetImageByUserIdQueryHandler> logger)
    {
        _blobService = blobService;
        _logger = logger;
    }

    public async Task<FileResponseDto> Handle(GetImageByUserIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting image for user ID: {UserId}", request.UserId);

        var user = await _userManager.FindByIdAsync(request.UserId.ToString());

        if (user is null)
        {
            _logger.LogWarning("User with ID {UserId} not found", request.UserId);
            
            throw new NotFoundException($"User with ID '{request.UserId}' not found");
        }

        if (user.ImageUrl is null)
        {
            _logger.LogWarning("User with ID {UserId} doesn't have an image", request.UserId);
            
            throw new NotFoundException($"User with ID {request.UserId} don't have an image");
        }

        _logger.LogInformation("Downloading image for user ID: {UserId}", request.UserId);
        
        var result = await _blobService.DownloadAsync(Guid.Parse(user.ImageUrl), cancellationToken);
        
        _logger.LogInformation("Successfully retrieved image for user ID: {UserId}", request.UserId);
        
        return result;
    }
}