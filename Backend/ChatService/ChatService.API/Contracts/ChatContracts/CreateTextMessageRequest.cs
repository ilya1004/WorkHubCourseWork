namespace ChatService.API.Contracts.ChatContracts;

public sealed record CreateTextMessageRequest(string ChatId, Guid ReceiverId, string Text);