namespace ChatService.Application.UseCases.MessageUseCases.Commands.DeleteMessage;

public sealed record DeleteMessageCommand(string MessageId) : IRequest;