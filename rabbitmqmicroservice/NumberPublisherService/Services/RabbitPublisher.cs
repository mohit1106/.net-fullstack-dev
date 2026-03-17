using System.Globalization;
using System.Text;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

public sealed class RabbitPublisher
{
    private readonly RabbitMqOptions _options;

    public RabbitPublisher(IOptions<RabbitMqOptions> options)
    {
        _options = options.Value;
    }

    public async Task<string> PublishNumberToQueuesAsync(int number, CancellationToken cancellationToken)
    {
        var message = number.ToString(CultureInfo.InvariantCulture);
        var body = Encoding.UTF8.GetBytes(message);
        var correlationId = Guid.NewGuid().ToString("N");
        var properties = new BasicProperties
        {
            CorrelationId = correlationId,
            ContentType = "text/plain"
        };

        var factory = new ConnectionFactory
        {
            HostName = _options.HostName,
            Port = _options.Port,
            UserName = _options.UserName,
            Password = _options.Password
        };

        await using var connection = await factory.CreateConnectionAsync(cancellationToken);
        await using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await channel.QueueDeclareAsync(_options.SquareQueueName, durable: false, exclusive: false, autoDelete: false,
            arguments: null, cancellationToken: cancellationToken);
        await channel.QueueDeclareAsync(_options.CubeQueueName, durable: false, exclusive: false, autoDelete: false,
            arguments: null, cancellationToken: cancellationToken);

        await channel.BasicPublishAsync(exchange: string.Empty, routingKey: _options.SquareQueueName,
            mandatory: false, basicProperties: properties, body: body, cancellationToken: cancellationToken);
        await channel.BasicPublishAsync(exchange: string.Empty, routingKey: _options.CubeQueueName,
            mandatory: false, basicProperties: properties, body: body, cancellationToken: cancellationToken);

        return correlationId;
    }
}
