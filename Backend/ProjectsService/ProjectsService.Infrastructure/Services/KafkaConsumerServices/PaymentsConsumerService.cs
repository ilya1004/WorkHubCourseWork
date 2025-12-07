using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using ProjectsService.Application.Exceptions;
using ProjectsService.Infrastructure.DTOs;

namespace ProjectsService.Infrastructure.Services.KafkaConsumerServices;

public class PaymentsConsumerService(
    IOptions<KafkaSettings> options,
    IServiceScopeFactory serviceScopeFactory,
    ILogger<PaymentsConsumerService> logger) : BackgroundService
{
    private IConsumer<Ignore, string> _consumer = null!;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Starting payments consumer service");

        var config = new ConsumerConfig
        {
            BootstrapServers = options.Value.BootstrapServers,
            GroupId = "payments_group",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        _consumer = new ConsumerBuilder<Ignore, string>(config).Build();

        _consumer.Subscribe(options.Value.PaymentIntentSavingTopic);

        logger.LogInformation("Subscribed to topic: {Topic}", options.Value.PaymentIntentSavingTopic);

        await Task.Run(() => ConsumeMessagesAsync(stoppingToken), stoppingToken);
    }

    private async Task ConsumeMessagesAsync(CancellationToken stoppingToken)
    {
        try
        {
            logger.LogInformation("Starting to consume payment messages");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = _consumer.Consume(stoppingToken);

                    logger.LogInformation("Received payment message with key: {Key}", result.Message.Key);

                    var dto = JsonSerializer.Deserialize<SavePaymentIntentIdDto>(result.Message.Value);

                    if (dto is null)
                    {
                        logger.LogError("Failed to deserialize payment message");

                        throw new BadRequestException("Error occurred during message deserialization");
                    }

                    logger.LogInformation("Processing payment message for project {ProjectId}", dto.ProjectId);

                    await ProcessMessageAsync(dto, stoppingToken);
                }
                catch (ConsumeException ex)
                {
                    logger.LogError(ex, "Kafka consume error: {Error}", ex.Error.Reason);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error processing payment message: {Message}", ex.Message);
                }
            }
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("Payments consumer service is stopping due to cancellation request");
        }
        finally
        {
            _consumer.Close();

            logger.LogInformation("Kafka consumer closed successfully");
        }
    }

    private async Task ProcessMessageAsync(SavePaymentIntentIdDto dto, CancellationToken stoppingToken)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var project = await unitOfWork.ProjectsRepository.GetByIdAsync(Guid.Parse(dto.ProjectId), stoppingToken);

        if (project is null)
        {
            logger.LogError("Project {ProjectId} not found", dto.ProjectId);
            throw new NotFoundException($"Project with ID '{dto.ProjectId}' not found");
        }

        project.PaymentIntentId = dto.PaymentIntentId;

        await unitOfWork.ProjectsRepository.UpdateAsync(project, stoppingToken);
    }
}