using ChatService.API.Contracts.ChatContracts;
using ChatService.API.HubInterfaces;
using ChatService.API.Hubs;
using ChatService.Application.UseCases.MessageUseCases.Commands.CreateFileMessage;
using ChatService.Application.UseCases.MessageUseCases.Queries.GetMessageFileById;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ChatService.API.Controllers;

[ApiController]
[Route("api/files")]
public class FilesController : ControllerBase
{
    private readonly IHubContext<ChatHub, IChatClient> _hubContext;
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public FilesController(
        IHubContext<ChatHub, IChatClient> hubContext,
        IMediator mediator,
        IMapper mapper)
    {
        _hubContext = hubContext;
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> UploadFile([FromForm] CreateFileMessageRequest request, 
        CancellationToken cancellationToken = default)
    {
        var message = await _mediator.Send(_mapper.Map<CreateFileMessageCommand>(request), cancellationToken);
        
        await _hubContext.Clients.User(request.ReceiverId.ToString()).ReceiveFileMessage(message);
        await _hubContext.Clients.User(message.SenderUserId.ToString()).ReceiveFileMessage(message);

        return Created();
    }
    
    [HttpGet]
    [Route("chat/{chatId}/file/{fileId:guid}")]
    [Authorize]
    public async Task<IActionResult> GetMessageFileById(string chatId, Guid fileId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetMessageFileByIdQuery(chatId, fileId), cancellationToken);
    
        return File(result.Stream, result.ContentType);
    }
}
