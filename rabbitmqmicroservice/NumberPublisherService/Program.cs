var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection(RabbitMqOptions.SectionName));
builder.Services.AddSingleton<RabbitPublisher>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("/api/publish", async (PublishNumberRequest request, RabbitPublisher publisher, CancellationToken cancellationToken) =>
{
    var correlationId = await publisher.PublishNumberToQueuesAsync(request.Number, cancellationToken);

    return Results.Ok(new
    {
        request.Number,
        CorrelationId = correlationId,
        Message = "Number published to square and cube queues."
    });
})
.WithName("PublishNumber");

app.Run();

public sealed record PublishNumberRequest(int Number);
