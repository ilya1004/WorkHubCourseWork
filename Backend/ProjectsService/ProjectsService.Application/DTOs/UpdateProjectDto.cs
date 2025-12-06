namespace ProjectsService.Application.DTOs;

public record UpdateProjectDto(
    string Description,
    decimal Budget,
    Guid? CategoryId);