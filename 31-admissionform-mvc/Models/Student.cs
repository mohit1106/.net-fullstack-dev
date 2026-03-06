using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Mail;

namespace _31_admissionform_mvc.Models;

/// <summary>
/// Student model for admission form with multi-level validation
/// - DataAnnotations layer
/// - DbContext Fluent API configuration
/// - SQL CHECK constraints via migrations
/// </summary>
public class Student : IValidatableObject
{
    [Key]
    public int StudentId { get; set; }

    [Required(ErrorMessage = "First name is required")]
    [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
    [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "First name must contain only letters")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required")]
    [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters")]
    [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Last name must contain only letters")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [StringLength(100)]
    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone number is required")]
    [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be exactly 10 digits")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Date of birth is required")]
    [DataType(DataType.Date)]
    public DateTime DateOfBirth { get; set; }

    [Required(ErrorMessage = "Gender is required")]
    [StringLength(10)]
    public string Gender { get; set; } = string.Empty;

    [Required(ErrorMessage = "Course is required")]
    [StringLength(100)]
    public string Course { get; set; } = string.Empty;

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime AdmissionDate { get; set; }

    public bool IsActive { get; set; } = true;

    // Custom validation for age (>= 15 years) - Implementation of IValidatableObject
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        // Check if DateOfBirth is in the future
        if (DateOfBirth > DateTime.Today)
        {
            yield return new ValidationResult(
                "Date of birth cannot be in the future",
                new[] { nameof(DateOfBirth) });
            yield break;
        }

        // Calculate age accurately
        var today = DateTime.Today;
        var age = today.Year - DateOfBirth.Year;

        // Adjust age if birthday hasn't occurred this year yet
        if (DateOfBirth.Date > today.AddYears(-age))
        {
            age--;
        }

        // Validate minimum age of 15 years
        if (age < 15)
        {
            yield return new ValidationResult(
                $"Student must be at least 15 years old. Current age: {age} years.",
                new[] { nameof(DateOfBirth) });
        }

        // Additional email format validation: ensure domain contains a dot (e.g., gmail.com)
        if (!string.IsNullOrWhiteSpace(Email))
        {
            bool emailValid = true;
            try
            {
                var mail = new MailAddress(Email);
                emailValid = mail.Host.Contains('.');
            }
            catch
            {
                emailValid = false;
            }

            if (!emailValid)
            {
                yield return new ValidationResult(
                    "Please enter a valid email address (include domain, e.g. user@example.com)",
                    new[] { nameof(Email) });
            }
        }
    }
}
