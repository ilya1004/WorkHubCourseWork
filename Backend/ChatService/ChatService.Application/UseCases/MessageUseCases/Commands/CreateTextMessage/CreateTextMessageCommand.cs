namespace ChatService.Application.UseCases.MessageUseCases.Commands.CreateTextMessage;

public sealed record CreateTextMessageCommand(
    string ChatId,
    Guid ReceiverId,
    string Text) : IRequest<Message>;