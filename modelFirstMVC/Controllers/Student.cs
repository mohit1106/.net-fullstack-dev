using Microsoft.AspNetCore.Mvc;
using modelFirstMVC.Data;
using modelFirstMVC.Models;
using System.Text;
using System.Linq;

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
        public IActionResult Create(string name, float age, string? department)
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
                sb.Append($"{s.Id} - {s.Name} - {s.Age} - {s.Department} - {s.City} <br>");
            }

            return Content(sb.ToString(), "text/html");
        }

        // GET: /Students/Details/1
        public IActionResult Details(int id)
        {
            var s = _context.Students.Find(id);

            if (s == null)
                return Content("Student not found");

            return Content($"{s.Id} - {s.Name} - {s.Age} - {s.Department} - {s.City}");
        }

        [HttpGet]
        public IActionResult Edit(int id, string name, float age, string city)
        {
            var student = _context.Students.Find(id);

            if (student == null)
                return Content("Student not found");

            student.Name = name;
            student.Age = age;
            student.City = city;

            _context.SaveChanges();

            return Content("Student Updated Successfully");
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var student = _context.Students.Find(id);

            if (student == null)
                return Content("Student not found");

            _context.Students.Remove(student);
            _context.SaveChanges();

            return Content("Student Deleted Successfully");
        }
    }
}