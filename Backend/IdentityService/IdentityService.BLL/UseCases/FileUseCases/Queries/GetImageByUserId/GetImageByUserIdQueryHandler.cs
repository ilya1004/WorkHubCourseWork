using IdentityService.BLL.Abstractions.BlobService;
using IdentityService.BLL.DTOs;

namespace IdentityService.BLL.UseCases.FileUseCases.Queries.GetImageByUserId;

public class GetImageByUserIdQueryHandler : IRequestHandler<GetImageByUserIdQuery, FileResponseDto>
{
    private readonly IBlobService _blobService;
    private readonly ILogger<GetImageByUserIdQueryHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public GetImageByUserIdQueryHandler(
        IBlobService blobService,
        ILogger<GetImageByUserIdQueryHandler> logger,
        IUnitOfWork unitOfWork)
    {
        _blobService = blobService;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<FileResponseDto> Handle(GetImageByUserIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.UsersRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            _logger.LogError("User with ID {UserId} not found", request.UserId);
            throw new NotFoundException($"User with ID '{request.UserId}' not found");
        }

        if (user.ImageUrl is null)
        {
            _logger.LogError("User with ID {UserId} doesn't have an image", request.UserId);
            throw new NotFoundException($"User with ID {request.UserId} don't have an image");
        }

        var result = await _blobService.DownloadAsync(Guid.Parse(user.ImageUrl), cancellationToken);

        return result;
    }
}