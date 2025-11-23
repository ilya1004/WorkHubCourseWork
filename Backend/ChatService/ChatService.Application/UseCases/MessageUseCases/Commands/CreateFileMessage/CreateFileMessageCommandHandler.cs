using ChatService.Domain.Abstractions.BlobService;

namespace ChatService.Application.UseCases.MessageUseCases.Commands.CreateFileMessage;

public class CreateFileMessageCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IUserContext userContext,
    IBlobService blobService,
    ILogger<CreateFileMessageCommandHandler> logger) : IRequestHandler<CreateFileMessageCommand, Message>
{
    public async Task<Message> Handle(CreateFileMessageCommand request, CancellationToken cancellationToken)
    {
        var chat = await unitOfWork.ChatRepository.GetByIdAsync(request.ChatId, cancellationToken);

        if (chat is null)
        {
            logger.LogError("Chat {ChatId} not found", request.ChatId);
            throw new NotFoundException($"Chat with ID '{request.ChatId}' not found");
        }
        
        var userId = userContext.GetUserId();

        if (chat.EmployerUserId != userId && chat.FreelancerUserId != userId)
        {
            logger.LogError("User {UserId} has no access to chat {ChatId}", userId, request.ChatId);
            throw new ForbiddenException($"You do not have access to chat with ID '{request.ChatId}'");
        }

        var message = mapper.Map<Message>(request);
        message.SenderUserId = userId;

        var fileId = await blobService.UploadAsync(request.FileStream, request.ContentType, cancellationToken);
        
        message.FileId = fileId;
        await unitOfWork.MessagesRepository.InsertAsync(message, cancellationToken);

        return message;
    }
}