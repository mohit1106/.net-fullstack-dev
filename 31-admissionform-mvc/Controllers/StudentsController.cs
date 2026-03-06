using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using _31_admissionform_mvc.Data;
using _31_admissionform_mvc.Models;

namespace _31_admissionform_mvc.Controllers;

/// <summary>
/// StudentsController - Handles full CRUD operations for students
/// - Index displays only active students
/// - Soft delete via IsActive flag
/// - Validation at controller and data layer
/// </summary>
public class StudentsController : Controller
{
    private readonly AdmissionDbContext _context;
    private readonly ILogger<StudentsController> _logger;

    // Constants for dropdown selections
    private static readonly List<string> Genders = new() { "Male", "Female", "Other" };
    private static readonly List<string> Courses = new() { "CSE", "ECE", "MECH", "CIVIL", "EEE" };

    public StudentsController(AdmissionDbContext context, ILogger<StudentsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: Students
    /// <summary>
    /// List all active students
    /// </summary>
    public async Task<IActionResult> Index()
    {
        try
        {
            var students = await _context.Students
                .Where(s => s.IsActive)
                .OrderBy(s => s.FirstName)
                .ThenBy(s => s.LastName)
                .ToListAsync();

            return View(students);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving students");
            TempData["ErrorMessage"] = "An error occurred while retrieving students.";
            return View(new List<Student>());
        }
    }

    // GET: Students/Details/5
    /// <summary>
    /// Display details of a specific student
    /// </summary>
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        try
        {
            var student = await _context.Students.FirstOrDefaultAsync(m => m.StudentId == id);
            
            if (student == null || !student.IsActive)
            {
                return NotFound();
            }

            return View(student);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving student details");
            return NotFound();
        }
    }

    // GET: Students/Create
    /// <summary>
    /// Display create student form
    /// </summary>
    public IActionResult Create()
    {
        ViewData["Genders"] = Genders;
        ViewData["Courses"] = Courses;
        return View();
    }

    // POST: Students/Create
    /// <summary>
    /// Create a new student record
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("FirstName,LastName,Email,Phone,DateOfBirth,Gender,Course")] Student student)
    {
        try
        {
            // Explicitly validate the model including IValidatableObject
            var validationContext = new ValidationContext(student);
            var validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(student, validationContext, validationResults, validateAllProperties: true);

            if (isValid && ModelState.IsValid)
            {
                // Additional server-side validation
                if (await _context.Students.AnyAsync(s => s.Email == student.Email && s.IsActive))
                {
                    ModelState.AddModelError("Email", "This email is already registered.");
                    ViewData["Genders"] = Genders;
                    ViewData["Courses"] = Courses;
                    return View(student);
                }

                // Enforce unique phone
                if (await _context.Students.AnyAsync(s => s.Phone == student.Phone && s.IsActive))
                {
                    ModelState.AddModelError("Phone", "This phone number is already registered.");
                    ViewData["Genders"] = Genders;
                    ViewData["Courses"] = Courses;
                    return View(student);
                }

                _context.Add(student);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation($"Student {student.FirstName} {student.LastName} created with ID {student.StudentId}");
                TempData["SuccessMessage"] = "Student record created successfully.";
                
                return RedirectToAction(nameof(Index));
            }
            else if (!isValid)
            {
                // Add custom validation errors to ModelState
                foreach (var validationResult in validationResults)
                {
                    foreach (var memberName in validationResult.MemberNames)
                    {
                        ModelState.AddModelError(memberName, validationResult.ErrorMessage);
                    }
                }
            }
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error while creating student");
            ModelState.AddModelError("", "A database error occurred. Please try again.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating student");
            ModelState.AddModelError("", "An error occurred while creating the record.");
        }

        ViewData["Genders"] = Genders;
        ViewData["Courses"] = Courses;
        return View(student);
    }

    // GET: Students/Edit/5
    /// <summary>
    /// Display edit student form
    /// </summary>
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        try
        {
            var student = await _context.Students.FindAsync(id);
            
            if (student == null || !student.IsActive)
            {
                return NotFound();
            }

            ViewData["Genders"] = Genders;
            ViewData["Courses"] = Courses;
            return View(student);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving student for edit");
            return NotFound();
        }
    }

    // POST: Students/Edit/5
    /// <summary>
    /// Update a student record
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("StudentId,FirstName,LastName,Email,Phone,DateOfBirth,Gender,Course,IsActive")] Student student)
    {
        if (id != student.StudentId)
        {
            return NotFound();
        }

        try
        {
            // Explicitly validate the model including IValidatableObject
            var validationContext = new ValidationContext(student);
            var validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(student, validationContext, validationResults, validateAllProperties: true);

            if (isValid && ModelState.IsValid)
            {
                // Check for duplicate email (excluding current student)
                if (await _context.Students.AnyAsync(s => 
                    s.Email == student.Email && 
                    s.StudentId != id && 
                    s.IsActive))
                {
                    ModelState.AddModelError("Email", "This email is already registered.");
                    ViewData["Genders"] = Genders;
                    ViewData["Courses"] = Courses;
                    return View(student);
                }
                // Check for duplicate phone (excluding current student)
                if (await _context.Students.AnyAsync(s =>
                    s.Phone == student.Phone &&
                    s.StudentId != id &&
                    s.IsActive))
                {
                    ModelState.AddModelError("Phone", "This phone number is already registered.");
                    ViewData["Genders"] = Genders;
                    ViewData["Courses"] = Courses;
                    return View(student);
                }

                _context.Update(student);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation($"Student {student.FirstName} {student.LastName} (ID: {id}) updated");
                TempData["SuccessMessage"] = "Student record updated successfully.";
                
                return RedirectToAction(nameof(Index));
            }
            else if (!isValid)
            {
                // Add custom validation errors to ModelState
                foreach (var validationResult in validationResults)
                {
                    foreach (var memberName in validationResult.MemberNames)
                    {
                        ModelState.AddModelError(memberName, validationResult.ErrorMessage);
                    }
                }
            }
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogError(ex, "Concurrency error while updating student");
            
            if (!await StudentExists(student.StudentId))
            {
                return NotFound();
            }
            
            ModelState.AddModelError("", "The record was modified by another user. Please refresh and try again.");
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error while updating student");
            ModelState.AddModelError("", "A database error occurred. Please try again.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating student");
            ModelState.AddModelError("", "An error occurred while updating the record.");
        }

        ViewData["Genders"] = Genders;
        ViewData["Courses"] = Courses;
        return View(student);
    }

    // GET: Students/Delete/5
    /// <summary>
    /// Display delete confirmation
    /// </summary>
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        try
        {
            var student = await _context.Students
                .FirstOrDefaultAsync(m => m.StudentId == id);
            
            if (student == null || !student.IsActive)
            {
                return NotFound();
            }

            return View(student);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving student for deletion");
            return NotFound();
        }
    }

    // POST: Students/Delete/5
    /// <summary>
    /// Soft delete a student (set IsActive to false)
    /// </summary>
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var student = await _context.Students.FindAsync(id);
            
            if (student == null)
            {
                return NotFound();
            }

            // Soft delete - set IsActive to false
            student.IsActive = false;
            _context.Update(student);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation($"Student {student.FirstName} {student.LastName} (ID: {id}) soft deleted");
            TempData["SuccessMessage"] = "Student record deleted successfully.";
            
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting student");
            TempData["ErrorMessage"] = "An error occurred while deleting the record.";
            return RedirectToAction(nameof(Index));
        }
    }

    private async Task<bool> StudentExists(int id)
    {
        return await _context.Students.AnyAsync(e => e.StudentId == id && e.IsActive);
    }
}
