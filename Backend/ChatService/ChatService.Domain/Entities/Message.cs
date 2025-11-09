using ChatService.Domain.Enums;

namespace ChatService.Domain.Entities;

public class Message
{
    public string Id { get; set; }
    public string? Text { get; set; }
    public Guid? FileId { get; set; }
    public Guid SenderUserId { get; set; }
    public Guid ReceiverUserId { get; set; }
    public Guid ChatId { get; set; }
    public MessageType Type { get; set; }
    public DateTime CreatedAt { get; set; }
}