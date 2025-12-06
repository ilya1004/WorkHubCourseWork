namespace ProjectsService.Application.UseCases.Commands.FreelancerApplicationUseCases.CreateFreelancerApplication;

public sealed record CreateFreelancerApplicationCommand(Guid ProjectId, Guid CvId) : IRequest;