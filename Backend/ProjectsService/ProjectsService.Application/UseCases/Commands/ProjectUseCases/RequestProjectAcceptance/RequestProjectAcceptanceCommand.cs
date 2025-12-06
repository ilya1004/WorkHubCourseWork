namespace ProjectsService.Application.UseCases.Commands.ProjectUseCases.UpdateAcceptanceRequest;

public sealed record RequestProjectAcceptanceCommand(Guid ProjectId) : IRequest;