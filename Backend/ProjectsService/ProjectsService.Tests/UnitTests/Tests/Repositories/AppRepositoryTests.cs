using Microsoft.EntityFrameworkCore;
using ProjectsService.Infrastructure.Data;
using ProjectsService.Infrastructure.Repositories;

namespace ProjectsService.Tests.UnitTests.Tests.Repositories;

public class AppRepositoryTests
{
    private readonly CommandsDbContext _context;
    private readonly AppRepository<Project> _repository;

    public AppRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<CommandsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        
        _context = new CommandsDbContext(options);
        _repository = new AppRepository<Project>(_context);
    }

    [Fact]
    public async Task AddAsync_AddsEntityToContext()
    {
        // Arrange
        var project = new Project
        {
            Id = Guid.NewGuid(), 
            Title = "Test Project",
            Description = "Test Description",
            Budget = 100m
        };

        // Act
        await _repository.AddAsync(project);
        await _context.SaveChangesAsync();

        // Assert
        var savedProject = await _context.Set<Project>().FindAsync(project.Id);
        savedProject.Should().BeEquivalentTo(project);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesEntityInContext()
    {
        // Arrange
        var project = new Project
        {
            Id = Guid.NewGuid(), 
            Title = "Initial Project",
            Description = "Test Description",
            Budget = 100m
        };
        await _context.Set<Project>().AddAsync(project);
        await _context.SaveChangesAsync();

        project.Title = "Updated Title";
        project.Description = "Updated Description";

        // Act
        await _repository.UpdateAsync(project);
        await _context.SaveChangesAsync();

        // Assert
        var updatedProject = await _context.Set<Project>().FindAsync(project.Id);
        updatedProject.Title.Should().Be("Updated Title");
        updatedProject.Title.Should().Be("Updated Description");
    }

    [Fact]
    public async Task DeleteAsync_RemovesEntityFromContext()
    {
        // Arrange
        var project = new Project
        {
            Id = Guid.NewGuid(), 
            Title = "Test Project",
            Description = "Test Description"
        };
        await _context.Set<Project>().AddAsync(project);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(project);
        await _context.SaveChangesAsync();

        // Assert
        var deletedProject = await _context.Set<Project>().FindAsync(project.Id);
        deletedProject.Should().BeNull();
    }

    [Fact]
    public async Task AddProject_WithoutTitle_ShouldThrowValidationException()
    {
        // Arrange
        var project = new Project
        {
            Id = Guid.NewGuid(),
            Title = null,
        };

        // Act
        var action = async () =>
        {
            await _context.Set<Project>().AddAsync(project);
            await _context.SaveChangesAsync();
        };

        // Assert
        await action.Should().ThrowAsync<DbUpdateException>();
    }
}