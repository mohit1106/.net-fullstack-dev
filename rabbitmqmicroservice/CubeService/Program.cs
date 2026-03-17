var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection(RabbitMqOptions.SectionName));
builder.Services.AddSingleton<CubeResultStore>();
builder.Services.AddHostedService<RabbitWorker>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/api/cube/latest", (CubeResultStore store) =>
{
    var latest = store.GetLatest();
    return latest is null ? Results.NotFound(new { Message = "No value consumed yet." }) : Results.Ok(latest);
})
.WithName("GetLatestCube");

app.Run();
