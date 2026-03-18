namespace StudentInsightsService.Models;

public sealed record StudentDto(int Id, string FirstName, string LastName, int Age, double Gpa, string Major);

public sealed record MajorCount(string Major, int Count);

public sealed record StudentInsightsResponse(
    int TotalStudents,
    double AverageGpa,
    int HonorRollCount,
    IReadOnlyList<MajorCount> StudentsByMajor);

public sealed record HonorRollStudent(int Id, string FullName, double Gpa, string Major);
