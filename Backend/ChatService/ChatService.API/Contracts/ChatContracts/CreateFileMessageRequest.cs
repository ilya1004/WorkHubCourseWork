namespace ChatService.API.Contracts.ChatContracts;

public sealed record CreateFileMessageRequest(string ChatId, Guid ReceiverId, IFormFile File);