using ChatService.Domain.Abstractions.BlobService;
using ChatService.Domain.Enums;

namespace ChatService.Application.UseCases.MessageUseCases.Commands.CreateFileMessage;

public class CreateFileMessageCommandHandler : IRequestHandler<CreateFileMessageCommand, Message>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;
    private readonly IBlobService _blobService;
    private readonly ILogger<CreateFileMessageCommandHandler> _logger;

    public CreateFileMessageCommandHandler(IUnitOfWork unitOfWork,
        IMapper mapper,
        IUserContext userContext,
        IBlobService blobService,
        ILogger<CreateFileMessageCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userContext = userContext;
        _blobService = blobService;
        _logger = logger;
    }

    public async Task<Message> Handle(CreateFileMessageCommand request, CancellationToken cancellationToken)
    {
        var chat = await _unitOfWork.ChatRepository.GetByIdAsync(request.ChatId, cancellationToken);

        if (chat is null)
        {
            _logger.LogError("Chat {ChatId} not found", request.ChatId);
            throw new NotFoundException($"Chat with ID '{request.ChatId}' not found");
        }
        
        var userId = _userContext.GetUserId();

        if (chat.EmployerUserId != userId && chat.FreelancerUserId != userId)
        {
            _logger.LogError("User {UserId} has no access to chat {ChatId}", userId, request.ChatId);
            throw new ForbiddenException($"You do not have access to chat with ID '{request.ChatId}'");
        }

        var fileId = await _blobService.UploadAsync(request.FileStream, request.ContentType, cancellationToken);

        var message = new Message
        {
            FileId = fileId,
            SenderUserId = userId,
            ReceiverUserId = request.ReceiverId,
            ChatId = request.ChatId,
            Type = MessageType.File,
            CreatedAt = DateTime.UtcNow,
        };

        await _unitOfWork.MessagesRepository.InsertAsync(message, cancellationToken);

        return message;
    }
}