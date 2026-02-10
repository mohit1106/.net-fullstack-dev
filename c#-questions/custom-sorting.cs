using System;
using System.Collections.Generic;

class Student
{
    public string Name { get; set; }
    public int Age { get; set; }
    public int Marks { get; set; }
}

class StudentComparer : IComparer<Student>
{
    public int Compare(Student a, Student b)
    {
        int marksCompare = b.Marks.CompareTo(a.Marks);
        if (marksCompare != 0) return marksCompare;
        return a.Age.CompareTo(b.Age);
    }
}

class Program
{
    static void Main()
    {
        var students = new List<Student>
        {
            new Student { Name = "A", Age = 21, Marks = 90 },
            new Student { Name = "B", Age = 19, Marks = 90 },
            new Student { Name = "C", Age = 20, Marks = 95 },
            new Student { Name = "D", Age = 22, Marks = 85 }
        };

        students.Sort(new StudentComparer());

        foreach (var s in students)
            Console.WriteLine($"{s.Name} {s.Age} {s.Marks}");
    }
}
