using StudentInsightsService.Models;
using StudentInsightsService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
var studentServiceBaseUrl = builder.Configuration["DownstreamServices:StudentServiceBaseUrl"]
    ?? throw new InvalidOperationException("Missing configuration: DownstreamServices:StudentServiceBaseUrl");

builder.Services.AddHttpClient<StudentServiceClient>(client =>
{
    client.BaseAddress = new Uri(studentServiceBaseUrl);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/api/insights/summary", async (StudentServiceClient client, CancellationToken cancellationToken) =>
{
    var students = await client.GetStudentsAsync(cancellationToken);
    var totalStudents = students.Count;
    var averageGpa = totalStudents == 0 ? 0 : Math.Round(students.Average(s => s.Gpa), 2);
    var honorRollCount = students.Count(s => s.Gpa >= 3.5);
    var studentsByMajor = students
        .GroupBy(s => s.Major)
        .Select(group => new MajorCount(group.Key, group.Count()))
        .OrderByDescending(item => item.Count)
        .ThenBy(item => item.Major)
        .ToList();

    var response = new StudentInsightsResponse(totalStudents, averageGpa, honorRollCount, studentsByMajor);
    return Results.Ok(response);
})
    .WithName("GetStudentInsightsSummary");

app.MapGet("/api/insights/honor-roll", async (StudentServiceClient client, CancellationToken cancellationToken) =>
{
    var students = await client.GetStudentsAsync(cancellationToken);
    var honorRoll = students
        .Where(student => student.Gpa >= 3.5)
        .OrderByDescending(student => student.Gpa)
        .ThenBy(student => student.LastName)
        .ThenBy(student => student.FirstName)
        .Select(student => new HonorRollStudent(student.Id, $"{student.FirstName} {student.LastName}", student.Gpa, student.Major))
        .ToList();

    return Results.Ok(honorRoll);
})
    .WithName("GetHonorRoll");

app.MapGet("/api/insights/top-student", async (StudentServiceClient client, CancellationToken cancellationToken) =>
{
    var students = await client.GetStudentsAsync(cancellationToken);
    var topStudent = students
        .OrderByDescending(student => student.Gpa)
        .ThenBy(student => student.LastName)
        .ThenBy(student => student.FirstName)
        .FirstOrDefault();

    if (topStudent is null)
    {
        return Results.NotFound(new { Message = "No students available from StudentService." });
    }

    return Results.Ok(new HonorRollStudent(topStudent.Id, $"{topStudent.FirstName} {topStudent.LastName}", topStudent.Gpa, topStudent.Major));
})
    .WithName("GetTopStudent");

app.Run();
