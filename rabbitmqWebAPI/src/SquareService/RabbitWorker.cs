using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace SquareService
{
    public class RabbitWorker : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                Port = 5672
            };

            var connection = await factory.CreateConnectionAsync();
            var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                queue: "number_queue",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            await channel.QueueDeclareAsync(
                queue: "square_queue",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            Console.WriteLine("SquareService waiting for messages...");

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (sender, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                int number = int.Parse(message);
                int square = number * number;

                Console.WriteLine($"Received: {number} → Square: {square}");

                var newBody = Encoding.UTF8.GetBytes(square.ToString());

                await channel.BasicPublishAsync(
                    exchange: "",
                    routingKey: "square_queue",
                    body: newBody);
            };

            await channel.BasicConsumeAsync(
                queue: "number_queue",
                autoAck: true,
                consumer: consumer);

            // Keep the worker alive until cancellation is requested
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
}
