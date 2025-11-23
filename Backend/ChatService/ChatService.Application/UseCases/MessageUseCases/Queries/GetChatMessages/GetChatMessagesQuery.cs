using ChatService.Application.Models;

namespace ChatService.Application.UseCases.MessageUseCases.Queries.GetChatMessages;

public sealed record GetChatMessagesQuery(string ChatId, int PageNo, int PageSize) : IRequest<PaginatedResultModel<Message>>;