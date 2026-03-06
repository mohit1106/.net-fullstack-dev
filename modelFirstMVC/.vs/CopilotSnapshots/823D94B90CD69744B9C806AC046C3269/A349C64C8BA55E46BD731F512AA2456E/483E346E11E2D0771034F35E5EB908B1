using Microsoft.AspNetCore.Mvc;
using modelFirstMVC.Data;
using modelFirstMVC.Models;
using System.Text;

namespace modelFirstMVC.Controllers
{
    public class StudentsController : Controller
    {
        private readonly StudentManagementContext _context;

        public StudentsController(StudentManagementContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Create(string name, int age, string? department)
        {
            var student = new Student
            {
                Name = name,
                Age = age,
                Department = department
            };

            _context.Students.Add(student);
            _context.SaveChanges();

            return Content("Student Created Successfully");
        }

        // GET: /Students/All
        public IActionResult All()
        {
            var students = _context.Students.ToList();
            StringBuilder sb = new StringBuilder();

            foreach (var s in students)
            {
                sb.Append($"{s.Id} - {s.Name} - {s.Age} - {s.Department} <br>");
            }

            return Content(sb.ToString(), "text/html");
        }

        // GET: /Students/Details/1
        public IActionResult Details(int id)
        {
            var s = _context.Students.Find(id);

            if (s == null)
                return Content("Student not found");

            return Content($"{s.Id} - {s.Name} - {s.Age} - {s.Department}");
        }
    }
}