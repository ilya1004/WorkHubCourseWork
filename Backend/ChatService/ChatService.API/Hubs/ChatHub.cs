using ChatService.API.Contracts.ChatContracts;
using ChatService.API.HubInterfaces;
using ChatService.Application.UseCases.ChatUseCases.Commands.CreateChat;
using ChatService.Application.UseCases.ChatUseCases.Commands.SetChatInactive;
using ChatService.Application.UseCases.ChatUseCases.Queries.GetChatById;
using ChatService.Application.UseCases.ChatUseCases.Queries.GetChatByProjectId;
using ChatService.Application.UseCases.MessageUseCases.Commands.CreateTextMessage;
using ChatService.Application.UseCases.MessageUseCases.Commands.DeleteMessage;
using ChatService.Application.UseCases.MessageUseCases.Queries.GetChatMessages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ChatService.API.Hubs;

public class ChatHub : Hub<IChatClient>
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly ILogger<ChatHub> _logger;

    public ChatHub(IMediator mediator,
        IMapper mapper,
        ILogger<ChatHub> logger)
    {
        _mediator = mediator;
        _mapper = mapper;
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("User {UserId} connected with connection ID {ConnectionId}", Context.UserIdentifier, Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("User {UserId} disconnected (Connection ID: {ConnectionId})", Context.UserIdentifier, Context.ConnectionId);
        
        if (exception is not null)
        {
            _logger.LogError(exception, "Disconnection error for user {UserId}", Context.UserIdentifier);
        }
        
        await base.OnDisconnectedAsync(exception);
    }
    
    [Authorize]
    public async Task CreateChat(CreateChatRequest request)
    {
        await _mediator.Send(_mapper.Map<CreateChatCommand>(request));
    }
    
    [Authorize]
    public async Task GetChatById(Guid chatId)
    {
        var result = await _mediator.Send(_mapper.Map<GetChatByIdQuery>(chatId));

        await Clients.Caller.ReceiveChat(result);
    }
    
    [Authorize]
    public async Task GetChatByProjectId(Guid chatId)
    {
        var result = await _mediator.Send(_mapper.Map<GetChatByProjectIdQuery>(chatId));
        
        await Clients.Caller.ReceiveChat(result);
    }

    [Authorize]
    public async Task SetChatInactive(SetChatInactiveRequest request)
    {
        await _mediator.Send(_mapper.Map<SetChatInactiveCommand>(request));
    }
    
    [Authorize]
    public async Task SendTextMessage(CreateTextMessageRequest request)
    {
        var message = await _mediator.Send(_mapper.Map<CreateTextMessageCommand>(request));

        await Clients.Caller.ReceiveTextMessage(message);
        await Clients.User(request.ReceiverId.ToString()).ReceiveTextMessage(message);
    }
    
    [Authorize]
    public async Task GetChatMessages(GetChatMessagesRequest request)
    {
        var result = await _mediator.Send(_mapper.Map<GetChatMessagesQuery>(request));

        await Clients.Caller.ReceiveChatMessages(result);
    }
    
    [Authorize]
    public async Task DeleteMessage(DeleteMessageRequest request)
    {
        await _mediator.Send(new DeleteMessageCommand(request.MessageId));

        await Clients.Caller.MessageIsDeleted(request.MessageId);
        await Clients.User(request.ReceiverId.ToString()).MessageIsDeleted(request.MessageId);
    }
}