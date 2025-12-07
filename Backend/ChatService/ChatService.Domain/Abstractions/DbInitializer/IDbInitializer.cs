namespace ChatService.Domain.Abstractions.DbInitializer;

public interface IDbInitializer
{
    Task InitializeDbAsync();
}