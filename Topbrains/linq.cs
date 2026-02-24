using System;
using System.Collections.Generic;
using System.Linq;

class Employee
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Department { get; set; }
    public int Salary { get; set; }

    public Employee(int id, string name, string department, int salary)
    {
        Id = id;
        Name = name;
        Department = department;
        Salary = salary;
    }
}

class Program
{
    static void Main()
    {
        List<Employee> employees = new List<Employee>
        {
            new Employee(1, "Arun", "IT", 60000),
            new Employee(2, "Meera", "HR", 45000),
            new Employee(3, "John", "IT", 75000)
        };

        Dictionary<string, List<Employee>> result = employees
            .Where(e => e.Salary > 50000)
            .GroupBy(e => e.Department)
            .ToDictionary(g => g.Key, g => g.ToList());

        foreach (var dept in result)
        {
            Console.WriteLine(dept.Key + " → " + string.Join(", ", dept.Value.Select(e => e.Name)));
        }
    }
}