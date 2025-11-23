namespace ChatService.Application.UseCases.MessageUseCases.Commands.CreateTextMessage;

public class CreateTextMessageCommandHandler(
    IUnitOfWork unitOfWork,
    IUserContext userContext,
    IMapper mapper,
    ILogger<CreateTextMessageCommandHandler> logger) : IRequestHandler<CreateTextMessageCommand, Message>
{
    public async Task<Message> Handle(CreateTextMessageCommand request, CancellationToken cancellationToken)
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
        
        await unitOfWork.MessagesRepository.InsertAsync(message, cancellationToken);

        return message;
    }
}