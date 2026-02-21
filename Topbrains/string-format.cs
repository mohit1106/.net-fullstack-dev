using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

public record Student(string Name, int Score);

public class Solution
{
    public string BuildAndSerialize(string[] items, int minScore)
    {
        if (items == null || items.Length == 0)
            return "[]";

        List<Student> students = new List<Student>();

        foreach (var item in items)
        {
            if (string.IsNullOrWhiteSpace(item))
                continue;

            var parts = item.Split(':');
            if (parts.Length != 2)
                continue;

            if (int.TryParse(parts[1], out int score))
            {
                students.Add(new Student(parts[0], score));
            }
        }

        var result = students
            .Where(s => s.Score >= minScore)
            .OrderByDescending(s => s.Score)
            .ThenBy(s => s.Name)
            .ToList();

        return JsonSerializer.Serialize(result);
    }
}
