namespace ChatService.Application.UseCases.ChatUseCases.Queries.GetChatById;

public sealed record GetChatByIdQuery(string Id) : IRequest<Chat>;