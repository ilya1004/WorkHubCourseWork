using ChatService.Domain.Abstractions.BlobService;

namespace ChatService.Application.UseCases.MessageUseCases.Queries.GetMessageFileById;

public class GetMessageFileByIdQueryHandler : IRequestHandler<GetMessageFileByIdQuery, FileResponse>
{
    private readonly IBlobService _blobService;
    private readonly ILogger<GetMessageFileByIdQueryHandler> _logger;
    private readonly IUserContext _userContext;
    private readonly IUnitOfWork _unitOfWork;

    public GetMessageFileByIdQueryHandler(IBlobService blobService,
        ILogger<GetMessageFileByIdQueryHandler> logger,
        IUserContext userContext,
        IUnitOfWork unitOfWork)
    {
        _blobService = blobService;
        _logger = logger;
        _userContext = userContext;
        _unitOfWork = unitOfWork;
    }

    public async Task<FileResponse> Handle(GetMessageFileByIdQuery request, CancellationToken cancellationToken)
    {
        var userId = _userContext.GetUserId();
        
        var chat = await _unitOfWork.ChatRepository.GetByIdAsync(request.ChatId, cancellationToken);

        if (chat is null)
        {
            _logger.LogError("Chat with ID '{ChatId}' not found", request.ChatId);
            throw new NotFoundException("Chat not found");
        }

        if (chat.FreelancerUserId != userId && chat.EmployerUserId != userId)
        {
            _logger.LogError("You do not have access to chat with ID '{ChatId}'", request.ChatId);
            throw new ForbiddenException($"You do not have access to this chat");
        }
        
        var result = await _blobService.DownloadAsync(request.FileId, cancellationToken);
        
        return result;
    }
}