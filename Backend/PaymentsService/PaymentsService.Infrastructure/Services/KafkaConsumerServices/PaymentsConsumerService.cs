using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using PaymentsService.Domain.Abstractions.PaymentsServices;

namespace PaymentsService.Infrastructure.Services.KafkaConsumerServices;

public class PaymentsConsumerService : BackgroundService
{
    private IConsumer<Ignore, string> _consumer = null!;
    private readonly IOptions<KafkaSettings> _options;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<PaymentsConsumerService> _logger;

    public PaymentsConsumerService(IOptions<KafkaSettings> options,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<PaymentsConsumerService> logger)
    {
        _options = options;
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting payments consumer service");
        
        var config = new ConsumerConfig
        {
            BootstrapServers = _options.Value.BootstrapServers,
            GroupId = "payments_group",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        _consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        _consumer.Subscribe(_options.Value.PaymentCancellationTopic);
        
        _logger.LogInformation("Subscribed to topic: {Topic}", _options.Value.PaymentCancellationTopic);

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
                 
                    _logger.LogInformation("Received message from Kafka. ProjectId: {ProjectId}", result.Message.Value);
                    
                    using var scope = _serviceScopeFactory.CreateScope();
                    var employerPaymentsService = scope.ServiceProvider.GetRequiredService<IEmployerPaymentsService>();

                    _logger.LogInformation("Processing payment cancellation for project: {ProjectId}", result.Message.Value);
                    
                    await employerPaymentsService.CancelPaymentIntentForProjectAsync(result.Message.Value, stoppingToken);
                    
                    _logger.LogInformation("Successfully processed payment cancellation for project: {ProjectId}", result.Message.Value);
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "Error consuming Kafka message. Error: {ErrorMessage}", ex.Error.Reason);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing payment cancellation. Error: {ErrorMessage}", ex.Message);
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Payments consumer service is stopping due to cancellation request");
        }
        finally
        {
            _consumer.Close();
            
            _logger.LogInformation("Kafka consumer closed successfully");
        }
    }
}