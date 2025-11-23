namespace ChatService.API.Contracts.ChatContracts;

public sealed record GetChatMessagesRequest(string ChatId, int PageNo, int PageSize);