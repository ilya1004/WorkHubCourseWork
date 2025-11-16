using IdentityService.BLL.Abstractions.BlobService;
using IdentityService.BLL.DTOs;

namespace IdentityService.BLL.UseCases.FileUseCases.Queries.GetImageById;

public class GetImageByIdQueryHandler : IRequestHandler<GetImageByIdQuery, FileResponseDto>
{
    private readonly IBlobService _blobService;

    public GetImageByIdQueryHandler(IBlobService blobService)
    {
        _blobService = blobService;
    }

    public async Task<FileResponseDto> Handle(GetImageByIdQuery request, CancellationToken cancellationToken)
    {
        var result = await _blobService.DownloadAsync(request.Id, cancellationToken);

        return result;
    }
}