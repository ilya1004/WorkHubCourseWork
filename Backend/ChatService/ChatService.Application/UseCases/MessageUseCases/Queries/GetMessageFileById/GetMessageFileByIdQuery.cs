using ChatService.Domain.Abstractions.BlobService;

namespace ChatService.Application.UseCases.MessageUseCases.Queries.GetMessageFileById;

public sealed record GetMessageFileByIdQuery(string ChatId, Guid FileId) : IRequest<FileResponse>;