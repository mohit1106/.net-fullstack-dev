# Admission Form MVC - Database Setup & Migration Instructions

## Project Overview

**31-admissionform-mvc** is a production-quality ASP.NET Core MVC application for student admission management built with:
- **.NET 10**
- **Entity Framework Core** (Code First approach)
- **SQL Server Express** database
- **Full validation** at 3 levels (DataAnnotations, DbContext Fluent API, SQL Constraints)
- **Soft delete** implementation using IsActive flag

---

## Database Configuration

### SQL Server Instance
```
Server: XI\SQLEXPRESS
Database: AdmissionDB
Connection String: Data Source=XI\SQLEXPRESS;Initial Catalog=AdmissionDB;Integrated Security=True;Encrypt=True;TrustServerCertificate=True
```

This is configured in `appsettings.json`:
```json
"ConnectionStrings": {
    "DefaultConnection": "Data Source=XI\\SQLEXPRESS;Initial Catalog=AdmissionDB;Integrated Security=True;Encrypt=True;TrustServerCertificate=True"
}
```

---

## Students Table Schema

The `Students` table has been created with the following structure:

| Column | Type | Constraints | Description |
|--------|------|-----------|-------------|
| **StudentId** | INT | PRIMARY KEY, IDENTITY(1,1) | Auto-incrementing primary key |
| **FirstName** | NVARCHAR(50) | NOT NULL | Student's first name (letters only) |
| **LastName** | NVARCHAR(50) | NOT NULL | Student's last name (letters only) |
| **Email** | NVARCHAR(100) | NOT NULL, UNIQUE | Must be valid email format |
| **Phone** | NVARCHAR(15) | NOT NULL | Exactly 10 digits |
| **DateOfBirth** | DATE | NOT NULL | Age must be >= 15 years |
| **Gender** | NVARCHAR(10) | NOT NULL, CHECK (Male/Female/Other) | One of three values |
| **Course** | NVARCHAR(100) | NOT NULL, CHECK (CSE/ECE/MECH/CIVIL/EEE) | One of five engineering courses |
| **AdmissionDate** | DATETIME2 | DEFAULT GETDATE() | Automatically set to current timestamp |
| **IsActive** | BIT | DEFAULT 1 | Soft delete flag (1=Active, 0=Deleted) |

### CHECK Constraints
```sql
CONSTRAINT [CK_Student_Gender] CHECK ([Gender] IN (N'Male', N'Female', N'Other'))
CONSTRAINT [CK_Student_Course] CHECK ([Course] IN (N'CSE', N'ECE', N'MECH', N'CIVIL', N'EEE'))
```

### UNIQUE Constraints
```sql
CREATE UNIQUE INDEX [UX_Student_Email] ON [Students] ([Email])
```

---

## Validation - 3-Level Enforcement

### Level 1: DataAnnotations (Client/Server Validation)
Located in `Models/Student.cs`:
- FirstName: Required, max 50 chars, letters only (regex)
- LastName: Required, max 50 chars, letters only (regex)
- Email: Required, valid email format
- Phone: Required, exactly 10 digits (regex)
- DateOfBirth: Required, custom validation for age >= 15
- Gender: Required dropdown selection
- Course: Required dropdown selection

### Level 2: DbContext Fluent API
Located in `Data/AdmissionDbContext.cs`:
- Column constraints via `HasMaxLength()`, `IsRequired()`
- UNIQUE index on Email via `HasIndex(...).IsUnique()`
- CHECK constraints for Gender and Course values
- Default values for AdmissionDate and IsActive

### Level 3: SQL Server CHECK Constraints
- `CK_Student_Gender`: Enforces valid gender values
- `CK_Student_Course`: Enforces valid course values
- These prevent invalid data entry at the database level

---

## Migration Files

### Initial Migration: `Data/Migrations/20260226044516_InitialCreate.cs`

Generated using Entity Framework Core's Code First approach. This migration:
1. Creates the `Students` table with all columns
2. Adds PRIMARY KEY constraint
3. Creates CHECK constraints for Gender and Course
4. Creates UNIQUE index on Email column
5. Sets default values for AdmissionDate and IsActive

---

## How to Run Migrations

### Prerequisites
- .NET 10 SDK installed
- SQL Server Express running
- VS Code with C# Dev Kit or Visual Studio

### First-Time Setup

#### 1. Verify Database Connection
Update `appsettings.json` if your SQL Server instance differs:
```json
"ConnectionStrings": {
    "DefaultConnection": "Data Source=YOUR_SERVER_NAME;Initial Catalog=AdmissionDB;Integrated Security=True;Encrypt=True;TrustServerCertificate=True"
}
```

#### 2. Apply Initial Migration
```bash
cd "d:\path\to\31-admissionform-mvc"
dotnet ef database update
```

This will:
- Create the `AdmissionDB` database (if it doesn't exist)
- Create the `Students` table
- Create all indexes and constraints
- Record migration history in `__EFMigrationsHistory` table

#### 3. Verify Database Creation
Connect to SQL Server and verify the `Students` table exists:
```sql
USE AdmissionDB;
SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Students';
```

---

## Managing Migrations

### Create a New Migration
After modifying the `Student` model in `Models/Student.cs`:

```bash
dotnet ef migrations add DescriptiveNameOfChange -o "Data/Migrations"
```

Example: Adding a date field
```bash
dotnet ef migrations add AddEnrollmentDateToStudent -o "Data/Migrations"
```

### View Migration History
```bash
dotnet ef migrations list
```

### Apply Pending Migrations
```bash
dotnet ef database update
```

### Revert to Previous Migration
```bash
dotnet ef database update PreviousMigrationName
```

Example:
```bash
dotnet ef database update 20260226044401_InitialCreate
```

### Remove Last Unapplied Migration
If you haven't applied it yet:
```bash
dotnet ef migrations remove
```

### Drop Database
⚠️ **Development only!** This deletes all data:
```bash
dotnet ef database drop --force
```

### Reset Database (Drop and Recreate)
```bash
dotnet ef database drop --force
dotnet ef database update
```

---

## Running the Application

### Development
```bash
cd "d:\Computer Eng\.net-fullstack-dev\31-admissionform-mvc"
dotnet run
```

Application will be available at: `https://localhost:5001` or `http://localhost:5000`

### Build for Release
```bash
dotnet publish -c Release -o .\publish
```

---

## CRUD Operations

### Create Student
- **Route**: `GET/POST /Students/Create`
- **Validation**: All 3 levels enforced
- **Dropdowns**: Gender (Male, Female, Other), Course (CSE, ECE, MECH, CIVIL, EEE)

### Read Students (Index)
- **Route**: `GET /Students`
- **Filter**: Shows only active students (IsActive = 1)
- **Soft Delete Ready**: Deleted records remain in DB with IsActive = 0

### Update Student
- **Route**: `GET/POST /Students/Edit/{id}`
- **Validation**: Email uniqueness checked (excluding current record)
- **Soft Delete Manage**: Can reactivate via IsActive checkbox

### Delete Student (Soft Delete)
- **Route**: `GET/POST /Students/Delete/{id}`
- **Action**: Sets IsActive = 0 (record remains in database)
- **View Impact**: Deleted records don't appear in Index list

### View Details
- **Route**: `GET /Students/Details/{id}`
- **Display**: All student information in read-only format

---

## Soft Delete Implementation

### How It Works
- When a student is deleted, `IsActive` is set to `0` instead of removing the record
- All queries filter by `IsActive = 1` to show only active students
- Data is preserved for audit trails and compliance

### Example Usage
```csharp
// Get active students only
var activeStudents = await _context.Students
    .Where(s => s.IsActive)
    .ToListAsync();

// Soft delete a student
var student = await _context.Students.FindAsync(id);
student.IsActive = false;
await _context.SaveChangesAsync();
```

---

## Entity Framework CLI Commands

### Install EF Core CLI (if needed)
```bash
dotnet tool install --global dotnet-ef
```

### Update EF Core Tools
```bash
dotnet tool update --global dotnet-ef
```

### Useful EF Commands
```bash
# Show EF version
dotnet ef --version

# Generate SQL for pending migrations (without applying)
dotnet ef migrations script

# Generate SQL from migration to migration
dotnet ef migrations script 20260226044516_InitialCreate 20260226044601_SecondMigration

# See what migrations have been applied
dotnet ef migrations list

# Get database provider info
dotnet ef dbcontext info
```

---

## Troubleshooting

### Error: "Unable to connect to SQL Server"
- ✅ Verify SQL Server Express is running
- ✅ Check server name in connection string
- ✅ Verify credentials and integrated authentication
- ✅ Test connection in SSMS

### Error: "There is already an object named 'Students' in the database"
- The table already exists. Either:
  - Use existing database: `dotnet ef database update`
  - Drop and recreate: `dotnet ef database drop --force && dotnet ef database update`

### Migrations not showing up
```bash
# Rebuild solution
dotnet build

# Check migrations folder exists
cd "d:\path\to\31-admissionform-mvc\Data\Migrations"
```

### Foreign key or constraint errors
- Recreate database from scratch:
  ```bash
  dotnet ef database drop --force
  dotnet ef database update
  ```

---

## Project Structure

```
31-admissionform-mvc/
├── Models/
│   ├── Student.cs                          # Student model with DataAnnotations
│   └── ErrorViewModel.cs
├── Data/
│   ├── AdmissionDbContext.cs               # DbContext with Fluent API configs
│   └── Migrations/
│       └── 20260226044516_InitialCreate.cs # Initial migration
├── Controllers/
│   └── StudentsController.cs               # Full CRUD operations
├── Views/
│   └── Students/
│       ├── Index.cshtml                    # List active students
│       ├── Create.cshtml                   # Create form with dropdowns
│       ├── Edit.cshtml                     # Edit form with validation
│       ├── Details.cshtml                  # View student info
│       └── Delete.cshtml                   # Delete confirmation
├── wwwroot/
│   └── css/
│       └── site.css                        # Production-quality styling
├── Program.cs                              # Dependency injection setup
├── appsettings.json                        # Connection string config
└── 31-admissionform-mvc.csproj             # Project file with EF packages
```

---

## Database Backup & Restore

### Backup AdmissionDB
```sql
BACKUP DATABASE [AdmissionDB] 
TO DISK = 'C:\Backups\AdmissionDB.bak' 
WITH INIT, COMPRESSION;
```

### Restore AdmissionDB
```sql
RESTORE DATABASE [AdmissionDB] 
FROM DISK = 'C:\Backups\AdmissionDB.bak';
```

---

## Production Considerations

1. **Connection String Security**
   - Use Azure Key Vault or environment variables for production
   - Never commit passwords to source control

2. **Encryption**
   - Enable TDE (Transparent Data Encryption) on SQL Server
   - Use HTTPS in production

3. **Backup Strategy**
   - Set up automated SQL Server backups
   - Test restore procedures regularly

4. **Monitoring**
   - Monitor application logs
   - Set up alerts for critical errors
   - Performance monitor database queries

5. **Compliance**
   - Audit trail via AdmissionDate timestamps
   - Soft delete for data retention
   - Consider GDPR implications for personal data

---

## Support & Resources

- **EF Core Documentation**: https://learn.microsoft.com/ef/core/
- **SQL Server Express**: https://www.microsoft.com/en-us/sql-server/sql-server-express
- **ASP.NET Core Docs**: https://learn.microsoft.com/aspnet/core/
- **Migration Issues**: https://aka.ms/efcore-docs-migrations-lock

---

## Developer Notes

This application is designed to be:
- ✅ Interview-ready with clean, professional code
- ✅ Production-quality with proper validation layers
- ✅ Maintainable with clear separation of concerns
- ✅ Extensible for future features and modifications
- ✅ Fully operational without authentication requirements

All constraints are enforced at three levels for data integrity and application resilience.
