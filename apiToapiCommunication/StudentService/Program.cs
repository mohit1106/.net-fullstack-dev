var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSingleton<IReadOnlyList<Student>>(
[
    new(1, "Ava", "Patel", 20, 3.9, "Computer Science"),
    new(2, "Noah", "Martinez", 22, 3.2, "Mechanical Engineering"),
    new(3, "Mia", "Johnson", 19, 3.7, "Mathematics"),
    new(4, "Liam", "Chen", 21, 2.9, "Physics"),
    new(5, "Sofia", "Ali", 23, 3.5, "Computer Science")
]);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/api/students", (IReadOnlyList<Student> students) => Results.Ok(students))
    .WithName("GetStudents");

app.MapGet("/api/students/{id:int}", (int id, IReadOnlyList<Student> students) =>
{
    var student = students.FirstOrDefault(s => s.Id == id);
    return student is null ? Results.NotFound() : Results.Ok(student);
})
    .WithName("GetStudentById");

app.Run();

public sealed record Student(int Id, string FirstName, string LastName, int Age, double Gpa, string Major);
