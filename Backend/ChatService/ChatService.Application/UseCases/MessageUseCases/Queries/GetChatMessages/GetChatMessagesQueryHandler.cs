using ChatService.Application.Models;

namespace ChatService.Application.UseCases.MessageUseCases.Queries.GetChatMessages;

public class GetChatMessagesQueryHandler : IRequestHandler<GetChatMessagesQuery, PaginatedResultModel<Message>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    private readonly ILogger<GetChatMessagesQueryHandler> _logger;

    public GetChatMessagesQueryHandler(IUnitOfWork unitOfWork,
        IUserContext userContext,
        ILogger<GetChatMessagesQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
        _logger = logger;
    }

    public async Task<PaginatedResultModel<Message>> Handle(GetChatMessagesQuery request, CancellationToken cancellationToken)
    {
        var chat = await _unitOfWork.ChatRepository.GetByIdAsync(request.ChatId, cancellationToken);

        if (chat is null)
        {
            _logger.LogWarning("Chat {ChatId} not found", request.ChatId);
            throw new NotFoundException($"Chat with ID '{request.ChatId}' not found");
        }
        
        var userId = _userContext.GetUserId();

        if (chat.EmployerUserId != userId && chat.FreelancerUserId != userId)
        {
            _logger.LogWarning("User {UserId} has no access to chat {ChatId}", userId, request.ChatId);
            throw new ForbiddenException($"You do not have access to chat with ID '{request.ChatId}'");
        }
        
        var offset = (request.PageNo - 1) * request.PageSize;
        
        var messages = await _unitOfWork.MessagesRepository.GetMessagesByChatIdAsync(
            request.ChatId, offset, request.PageSize, cancellationToken);

        var messagesCount = await _unitOfWork.MessagesRepository.CountAsync(
            m => m.ChatId == request.ChatId, cancellationToken);

        return new PaginatedResultModel<Message>
        {
            Items = messages.ToList(),
            PageNo = request.PageNo,
            PageSize = request.PageSize,
            TotalCount = messagesCount
        };
    }
}