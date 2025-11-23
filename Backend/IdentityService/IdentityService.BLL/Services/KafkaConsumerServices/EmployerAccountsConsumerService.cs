using System.Text.Json;
using Confluent.Kafka;
using IdentityService.BLL.DTOs;
using IdentityService.BLL.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IdentityService.BLL.Services.KafkaConsumerServices;

public class EmployerAccountsConsumerService : BackgroundService
{
    private IConsumer<Ignore, string> _consumer = null!;
    private readonly IOptions<KafkaSettings> _options;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<EmployerAccountsConsumerService> _logger;

    public EmployerAccountsConsumerService(IOptions<KafkaSettings> options,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<EmployerAccountsConsumerService> logger)
    {
        _options = options;
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _options.Value.BootstrapServers,
            GroupId = "accounts_group",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        _consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        _consumer.Subscribe(_options.Value.EmployerAccountIdSavingTopic);

        _logger.LogInformation("Subscribed to topic: {Topic}", _options.Value.EmployerAccountIdSavingTopic);
        
        await Task.Run(() => ConsumeMessagesAsync(stoppingToken), stoppingToken);
    }

    private async Task ConsumeMessagesAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = _consumer.Consume(stoppingToken);

                    _logger.LogInformation("Received message: {Message}", result.Message.Value);
                    
                    var dto = JsonSerializer.Deserialize<SaveEmployerAccountIdDto>(result.Message.Value);
                    
                    if (dto is null)
                    {
                        _logger.LogError("Failed to deserialize message: {Message}", result.Message.Value);
                        throw new BadRequestException("Error occurred during message deserialization");
                    }

                    await ProcessMessageAsync(dto, stoppingToken);
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "Kafka consume error");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing message");
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Consumer service stopping");
        }
        finally
        {
            _consumer.Close();
            
            _logger.LogInformation("Consumer closed");
        }
    }

    private async Task ProcessMessageAsync(SaveEmployerAccountIdDto dto, CancellationToken stoppingToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var user = await unitOfWork.UsersRepository.GetEmployerByIdAsync(
            Guid.Parse(dto.UserId), stoppingToken);

        if (user is null)
        {
            _logger.LogError("Employer user not found for user {UserId}", dto.UserId);
            throw new BadRequestException($"Employer user with user ID '{dto.UserId}' not found");
        }

        await unitOfWork.EmployerProfilesRepository.UpdateStripeCustomerIdAsync(user.Id, dto.EmployerAccountId, stoppingToken);
    }
}