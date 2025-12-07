using ChatService.Domain.Abstractions.DbInitializer;
using ChatService.Infrastructure.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ChatService.Infrastructure.Services.DbInitializer;

public class DbInitializer : IDbInitializer
{
    private readonly IOptions<MongoDbSettings> _options;
    private readonly ILogger<DbInitializer> _logger;

    public DbInitializer(
        IOptions<MongoDbSettings> options,
        ILogger<DbInitializer> logger)
    {
        _options = options;
        _logger = logger;
    }

    public async Task InitializeDbAsync()
    {
        try
        {
            var client = new MongoClient(_options.Value.ConnectionString);
            var database = client.GetDatabase(_options.Value.DatabaseName);

            var collections = (await database.ListCollectionNamesAsync()).ToList();

            if (!collections.Contains("Chats"))
            {
                _logger.LogInformation("Creating 'Chats' collection");
                await database.CreateCollectionAsync("Chats");
            }

            if (!collections.Contains("Messages"))
            {
                _logger.LogInformation("Creating 'Messages' collection");
                await database.CreateCollectionAsync("Messages");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database initialization failed");
            throw new Exception("Database initialization failed", ex);
        }
    }
}