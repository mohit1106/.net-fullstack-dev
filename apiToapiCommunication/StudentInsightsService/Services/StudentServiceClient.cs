using System.Net.Http.Json;
using StudentInsightsService.Models;

namespace StudentInsightsService.Services;

public sealed class StudentServiceClient(HttpClient httpClient)
{
    public async Task<IReadOnlyList<StudentDto>> GetStudentsAsync(CancellationToken cancellationToken = default)
    {
        var students = await httpClient.GetFromJsonAsync<List<StudentDto>>("/api/students", cancellationToken);
        return students ?? [];
    }
}
