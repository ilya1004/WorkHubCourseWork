namespace ChatService.Application.UseCases.MessageUseCases.Commands.CreateFileMessage;

public sealed record CreateFileMessageCommand(
    string ChatId,
    Guid ReceiverId,
    Stream FileStream,
    string ContentType) : IRequest<Message>;