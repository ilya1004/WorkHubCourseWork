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
            _logger.LogInformation("Starting to consume messages");
            
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = _consumer.Consume(stoppingToken);
                    _logger.LogInformation("Received message: {Message}", result.Message.Value);
                    
                    var dto = JsonSerializer.Deserialize<SaveEmployerAccountIdDto>(result.Message.Value);
                    
                    if (dto is null)
                    {
                        _logger.LogWarning("Failed to deserialize message: {Message}", result.Message.Value);
                        
                        throw new BadRequestException("Error occurred during message deserialization");
                    }

                    await ProcessMessageAsync(dto, stoppingToken);
                    
                    _logger.LogInformation("Successfully processed message for user {UserId}", dto.UserId);
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
        _logger.LogInformation("Processing employer account ID for user {UserId}", dto.UserId);
        
        using var scope = _serviceScopeFactory.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var employerProfile = await unitOfWork.EmployerProfilesRepository.FirstOrDefaultAsync(
            ep => ep.UserId == Guid.Parse(dto.UserId), stoppingToken);

        if (employerProfile is null)
        {
            _logger.LogWarning("Employer profile not found for user {UserId}", dto.UserId);
            
            throw new BadRequestException($"Employer profile with user ID '{dto.UserId}' not found");
        }

        employerProfile.StripeCustomerId = dto.EmployerAccountId;

        await unitOfWork.EmployerProfilesRepository.UpdateAsync(employerProfile, stoppingToken);
        await unitOfWork.SaveAllAsync(stoppingToken);
        
        _logger.LogInformation("Successfully updated employer profile for user {UserId}", dto.UserId);
    }
}