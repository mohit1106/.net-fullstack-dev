using System.Text;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

public sealed class RabbitWorker : BackgroundService
{
    private readonly RabbitMqOptions _options;
    private readonly SquareResultStore _resultStore;
    private readonly ILogger<RabbitWorker> _logger;
    private IConnection? _connection;
    private IChannel? _channel;

    public RabbitWorker(IOptions<RabbitMqOptions> options, SquareResultStore resultStore, ILogger<RabbitWorker> logger)
    {
        _options = options.Value;
        _resultStore = resultStore;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _options.HostName,
            Port = _options.Port,
            UserName = _options.UserName,
            Password = _options.Password
        };

        _connection = await factory.CreateConnectionAsync(stoppingToken);
        _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

        await _channel.QueueDeclareAsync(_options.SquareQueueName, durable: false, exclusive: false, autoDelete: false,
            arguments: null, cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (_, ea) =>
        {
            var payload = Encoding.UTF8.GetString(ea.Body.ToArray());
            var correlationId = ea.BasicProperties?.CorrelationId ?? "missing-correlation-id";

            if (!int.TryParse(payload, out var input))
            {
                _logger.LogWarning("Invalid payload consumed from queue: {Payload}. CorrelationId={CorrelationId}", payload,
                    correlationId);
                return;
            }

            var output = checked(input * input);
            _resultStore.SetLatest(input, output, correlationId);
            _logger.LogInformation("SquareService processed input {Input} -> {Output}. CorrelationId={CorrelationId}",
                input, output, correlationId);
            await Task.CompletedTask;
        };

        await _channel.BasicConsumeAsync(_options.SquareQueueName, autoAck: true, consumer: consumer,
            cancellationToken: stoppingToken);

        try
        {
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel is not null)
        {
            await _channel.DisposeAsync();
        }

        if (_connection is not null)
        {
            await _connection.DisposeAsync();
        }

        await base.StopAsync(cancellationToken);
    }
}
