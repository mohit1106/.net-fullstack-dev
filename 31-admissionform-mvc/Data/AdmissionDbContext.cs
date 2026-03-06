using Microsoft.EntityFrameworkCore;
using _31_admissionform_mvc.Models;

namespace _31_admissionform_mvc.Data;

/// <summary>
/// AdmissionDbContext - Manages database operations and enforces constraints via Fluent API
/// Constraints enforced:
/// 1. DataAnnotations (in Student.cs)
/// 2. Fluent API configuration (this file)
/// 3. SQL CHECK/UNIQUE constraints (via migrations)
/// </summary>
public class AdmissionDbContext : DbContext
{
    public AdmissionDbContext(DbContextOptions<AdmissionDbContext> options) 
        : base(options)
    {
    }

    public DbSet<Student> Students { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Student entity
        var studentEntity = modelBuilder.Entity<Student>();

        // Table configuration with CHECK constraints in SQL
        studentEntity.ToTable("Students", tb =>
        {
            // Gender CHECK constraint
            tb.HasCheckConstraint("CK_Student_Gender", "[Gender] IN (N'Male', N'Female', N'Other')");
            // Course CHECK constraint
            tb.HasCheckConstraint("CK_Student_Course", "[Course] IN (N'CSE', N'ECE', N'MECH', N'CIVIL', N'EEE')");
        });

        // Primary Key
        studentEntity.HasKey(s => s.StudentId);

        // Required properties
        studentEntity.Property(s => s.FirstName)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnType("nvarchar(50)");

        studentEntity.Property(s => s.LastName)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnType("nvarchar(50)");

        studentEntity.Property(s => s.Email)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnType("nvarchar(100)");

        studentEntity.Property(s => s.Phone)
            .IsRequired()
            .HasMaxLength(15)
            .HasColumnType("nvarchar(15)");

        studentEntity.Property(s => s.DateOfBirth)
            .IsRequired()
            .HasColumnType("date");

        studentEntity.Property(s => s.Gender)
            .IsRequired()
            .HasMaxLength(10)
            .HasColumnType("nvarchar(10)");

        studentEntity.Property(s => s.Course)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnType("nvarchar(100)");

        // UNIQUE constraint on Email
        studentEntity.HasIndex(s => s.Email)
            .IsUnique()
            .HasDatabaseName("UX_Student_Email");

        // UNIQUE constraint on Phone
        studentEntity.HasIndex(s => s.Phone)
            .IsUnique()
            .HasDatabaseName("UX_Student_Phone");

        // AdmissionDate defaults to GETDATE() in SQL Server
        studentEntity.Property(s => s.AdmissionDate)
            .HasDefaultValueSql("GETDATE()")
            .HasColumnType("datetime2")
            .ValueGeneratedOnAdd();

        // IsActive defaults to true
        studentEntity.Property(s => s.IsActive)
            .HasDefaultValue(true)
            .HasColumnType("bit");
    }
}
