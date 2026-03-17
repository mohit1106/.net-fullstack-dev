using Microsoft.AspNetCore.Mvc;
using WebAPIFromBody;

[ApiController]
[Route("api/[controller]")]
public class EmployeeController : ControllerBase
{
    // Static list of employees with some default data
    static List<Employee> employees = new List<Employee>
    {
        new Employee { Id = 1, Name = "Rauhan", Age = 25, Salary = 30000 },
        new Employee { Id = 2, Name = "Deepak", Age = 28, Salary = 35000 },
        new Employee { Id = 3, Name = "Mohit", Age = 24, Salary = 32000 },
        new Employee { Id = 4, Name = "Shivansh", Age = 26, Salary = 28000 }
    };

    // POST: api/employee/add - Add array of employees from body
    [HttpPost("add")]
    public IActionResult AddEmployees([FromBody] List<Employee> newEmployees)
    {
        foreach (var emp in newEmployees)
        {
            employees.Add(emp);
        }

        string message = newEmployees.Count + " employees added successfully.";
        return Ok(message);
    }

    // GET: api/employee/all - Get all employees
    [HttpGet("all")]
    public IActionResult GetAllEmployees()
    {
        return Ok(employees);
    }

    // GET: api/employee/totalsalary - Get total salary of all employees
    [HttpGet("totalsalary")]
    public IActionResult GetTotalSalary()
    {
        double totalSalary = 0;

        foreach (var emp in employees)
        {
            totalSalary = totalSalary + emp.Salary;
        }

        string message = "Total Salary of all employees: " + totalSalary;
        return Ok(message);
    }
}
