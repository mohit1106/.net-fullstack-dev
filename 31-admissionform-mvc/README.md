# 31-admissionform-mvc - ASP.NET Core Admission Form Application

A production-quality **ASP.NET Core MVC** application for managing student admissions with comprehensive validation, soft-delete functionality, and a clean, professional user interface.

## 🎯 Project Overview

Built with **modern technologies** and **best practices**:
- **.NET 10** - Latest long-term support framework
- **ASP.NET Core MVC** - Server-side rendering with clean architecture
- **Entity Framework Core** - Code First database approach
- **SQL Server Express** - Reliable relational database
- **3-Level Validation** - DataAnnotations, DbContext constraints, SQL checks
- **Soft Delete** - Audit-friendly record management

---

## ✨ Features

### Student Management
- ✅ **Create** new admission records with form validation
- ✅ **Read** active student records with pagination-ready table
- ✅ **Update** student information with conflict detection
- ✅ **Delete** (soft delete) students while preserving data
- ✅ **View Details** of individual student profiles

### Validation & Constraints
- ✅ **DataAnnotations** - Client-side and server-side validation
- ✅ **Business Rules** - Email uniqueness, age requirements (≥15 years)
- ✅ **Database Constraints** - CHECK constraints enforced at SQL level
- ✅ **Dropdown Selection** - Gender (Male/Female/Other), Course (5 engineering fields)
- ✅ **Format Validation** - Name letters only, phone exactly 10 digits, valid email

### User Experience
- ✅ **Responsive Design** - Works on desktop and mobile devices
- ✅ **Clean UI** - Professional styling with consistent branding
- ✅ **Error Messages** - Clear feedback for validation failures
- ✅ **Success Notifications** - Confirmation messages for operations
- ✅ **Intuitive Navigation** - Simple and logical workflow

### Data Integrity
- ✅ **UNIQUE Index** on Email column
- ✅ **CHECK Constraints** for Gender and Course values
- ✅ **NOT NULL** constraints for required fields
- ✅ **DEFAULT Values** for auto-generated fields
- ✅ **Soft Delete** tracking via IsActive flag

---

## 📋 Database Schema

### Students Table

| Column | Type | Constraints | Notes |
|--------|------|-----------|-------|
| StudentId | int | PK, IDENTITY | Auto-incrementing |
| FirstName | nvarchar(50) | NOT NULL | Letters only |
| LastName | nvarchar(50) | NOT NULL | Letters only |
| Email | nvarchar(100) | NOT NULL, UNIQUE | Valid format required |
| Phone | nvarchar(15) | NOT NULL | Exactly 10 digits |
| DateOfBirth | date | NOT NULL | Age ≥ 15 years |
| Gender | nvarchar(10) | NOT NULL, CHECK | Male/Female/Other |
| Course | nvarchar(100) | NOT NULL, CHECK | CSE/ECE/MECH/CIVIL/EEE |
| AdmissionDate | datetime2 | DEFAULT GETDATE() | System timestamp |
| IsActive | bit | DEFAULT 1 | Soft delete flag |

### CHECK Constraints
```sql
CK_Student_Gender: [Gender] IN ('Male', 'Female', 'Other')
CK_Student_Course: [Course] IN ('CSE', 'ECE', 'MECH', 'CIVIL', 'EEE')
```

---

## 🔧 Technical Stack

| Component | Technology | Version |
|-----------|-----------|---------|
| Framework | ASP.NET Core MVC | .NET 10 |
| ORM | Entity Framework Core | 10.0.3 |
| Database | SQL Server Express | Latest |
| Language | C# | 13 |
| Frontend | Razor Views + CSS3 | Responsive |
| Validation | DataAnnotations+Custom | Client & Server |

---

## 🚀 Getting Started

### Prerequisites
- **.NET 10 SDK** - [Download](https://dotnet.microsoft.com/download)
- **SQL Server Express** - [Download](https://www.microsoft.com/sql-server/sql-server-express)
- **VS Code** or **Visual Studio** with C# extension

### Installation

#### 1. Clone or Open Project
```bash
cd "d:\Computer Eng\.net-fullstack-dev\31-admissionform-mvc"
```

#### 2. Verify Database Connection
Update `appsettings.json` if needed:
```json
"ConnectionStrings": {
    "DefaultConnection": "Data Source=XI\\SQLEXPRESS;Initial Catalog=AdmissionDB;Integrated Security=True;Encrypt=True;TrustServerCertificate=True"
}
```

#### 3. Apply Database Migration
```bash
dotnet ef database update
```

This will:
- Create `AdmissionDB` database
- Create `Students` table with all constraints
- Set up migration history tracking

#### 4. Run Application
```bash
dotnet run
```

Application will be available at:
- **HTTP**: http://localhost:5000
- **HTTPS**: https://localhost:5001

---

## 📁 Project Structure

```
31-admissionform-mvc/
│
├── Models/
│   ├── Student.cs                    ✓ Student entity with DataAnnotations
│   └── ErrorViewModel.cs
│
├── Data/
│   ├── AdmissionDbContext.cs         ✓ DbContext with Fluent API config
│   └── Migrations/
│       ├── 20260226044516_InitialCreate.cs
│       └── 20260226044516_InitialCreateDesigner.cs
│
├── Controllers/
│   ├── StudentsController.cs         ✓ Full CRUD operations
│   └── HomeController.cs
│
├── Views/
│   ├── Shared/
│   │   ├── _Layout.cshtml
│   │   ├── _ValidationScriptsPartial.cshtml
│   │   └── Error.cshtml
│   │
│   └── Students/                     ✓ Student management views
│       ├── Index.cshtml              (List & manage students)
│       ├── Create.cshtml             (Add new student)
│       ├── Edit.cshtml               (Update student info)
│       ├── Details.cshtml            (View student profile)
│       └── Delete.cshtml             (Confirm deletion)
│
├── wwwroot/
│   ├── css/
│   │   └── site.css                  ✓ Production styling
│   ├── js/
│   │   └── site.js
│   └── lib/                          (Bootstrap, jQuery, etc.)
│
├── Program.cs                        ✓ Dependency injection & config
├── appsettings.json                  ✓ Connection string
├── appsettings.Development.json
├── 31-admissionform-mvc.csproj       ✓ NuGet packages
├── 31-admissionform-mvc.sln          ✓ Solution file
├── MIGRATION_INSTRUCTIONS.md         ✓ Database setup guide
└── README.md                         ✓ This file
```

---

## 🎮 Usage Guide

### Adding a New Student

1. **Go to Students List**
   - Click "Add New Student" button or navigate to `/Students/Create`

2. **Fill in Form**
   - Enter name (letters only)
   - Valid email address
   - Phone (10 digits)
   - Date of birth (must be ≥15 years old)
   - Select gender and course from dropdowns

3. **Submit**
   - Click "Create Student" button
   - Server validates all constraints
   - Record added to database

4. **Confirmation**
   - Success message displayed
   - Redirected to student list
   - New record appears in table

### Viewing Students

1. **Access List**
   - Navigate to `/Students` or click "Students" menu
   - Only active students displayed (IsActive = 1)

2. **Search & Filter**
   - Table sorted by first name, last name
   - Can enhance with search functionality

3. **Inline Actions**
   - 👁️ View – See full student details
   - ✏️ Edit – Modify student information
   - 🗑️ Delete – Remove student from active list

### Editing Student

1. **Click Edit Button**
   - Opens student edit form with current data
   - All fields pre-populated

2. **Modify Information**
   - Update any field with new values
   - Check/uncheck "Active Student" to reactivate
   - Validation applies to changes

3. **Save Changes**
   - Click "Update Student"
   - Database verifies constraints
   - Redirected to list on success

### Deleting Student

1. **Click Delete Button**
   - Confirmation dialog shown
   - Displays warning about data preservation

2. **Confirm Deletion**
   - Click "Yes, Delete This Student"
   - Record marked as inactive (IsActive = 0)
   - Data preserved in database

3. **Soft Delete**
   - Deleted records don't appear in list
   - Can be reactivated via database if needed
   - Audit trail maintained

### Viewing Student Details

1. **Click View Button (👁️)**
   - Opens read-only student profile
   - Displays all information formatted nicely
   - Shows admission date and status

2. **Actions Available**
   - Edit student information
   - Delete student record
   - Return to student list

---

## 🔍 Validation Rules

### FirstName & LastName
- ✓ Required
- ✓ Maximum 50 characters
- ✓ Letters only (A-Z, a-z)
- ✗ No numbers or special characters

### Email
- ✓ Required
- ✓ Valid format (user@domain.com)
- ✓ Unique across all active students
- ✗ No duplicate emails allowed

### Phone
- ✓ Required
- ✓ Exactly 10 digits
- ✓ Numeric only
- ✗ No hyphens or spaces

### Date of Birth
- ✓ Required
- ✓ Age must be ≥ 15 years old
- ✗ Future dates not allowed
- ✗ Very old dates may be rejected on input

### Gender
- ✓ Required
- ✓ Must select from dropdown
- ✓ Valid values: Male, Female, Other

### Course
- ✓ Required
- ✓ Must select from dropdown
- ✓ Valid values: CSE, ECE, MECH, CIVIL, EEE

---

## 📊 Database Operations

### Initial Setup (First Time)
```bash
# Create migration
dotnet ef migrations add InitialCreate -o "Data/Migrations"

# Apply migration
dotnet ef database update
```

### Adding New Fields
```bash
# After modifying Student.cs model:
dotnet ef migrations add AddNewField -o "Data/Migrations"
dotnet ef database update
```

### Database Management
```bash
# View migration history
dotnet ef migrations list

# Revert to previous migration
dotnet ef database update PreviousMigrationName

# Drop database (development only)
dotnet ef database drop --force
```

See [MIGRATION_INSTRUCTIONS.md](MIGRATION_INSTRUCTIONS.md) for detailed migration guide.

---

## 🧪 Testing the Application

### Manual Testing Checklist

#### Create Student
- [ ] All fields required
- [ ] Name validation (letters only)
- [ ] Email uniqueness enforced
- [ ] Phone format validated (10 digits)
- [ ] Age validation works (≥15 years)
- [ ] Dropdowns work correctly
- [ ] Success message appears

#### Edit Student
- [ ] Form loads with current data
- [ ] Can modify any field
- [ ] Email uniqueness still enforced
- [ ] Validation applies to changes
- [ ] Updates reflected in database

#### Delete Student
- [ ] Confirmation dialog shown
- [ ] Record marked inactive (soft delete)
- [ ] Still exists in database
- [ ] Doesn't appear in list

#### View Details
- [ ] All information displayed
- [ ] Email is clickable link
- [ ] Course shown as badge
- [ ] Status indicator correct

---

## 🔐 Security Considerations

### Input Validation
- ✓ All inputs validated on server-side
- ✓ Client-side validation for UX
- ✓ Regular expressions enforce format
- ✓ SQL injection prevention via EF Core parameters

### Database Security
- ✓ Integrated Windows Authentication used
- ✓ Connection string with encryption flag
- ✓ UNIQUE constraints at database level
- ✓ CHECK constraints prevent invalid data

### Production Deployment
- [ ] Use environment variables for connection strings
- [ ] Never commit passwords to source control
- [ ] Enable database backups
- [ ] Monitor application logs
- [ ] Use HTTPS exclusively
- [ ] Implement rate limiting
- [ ] Add authentication/authorization for sensitive operations

---

## 📝 Code Quality

### Architecture
- ✓ **Clean Separation of Concerns** - Controllers, Models, Views, Data Access
- ✓ **Dependency Injection** - DbContext injected via DI container
- ✓ **Logging** - ILogger used for debugging and monitoring
- ✓ **Error Handling** - Try-catch with user-friendly messages
- ✓ **SOLID Principles** - Single responsibility, dependency inversion

### Best Practices
- ✓ **Entity Framework Core** - ORM for data access
- ✓ **DbContext Fluent API** - Configuration at mapping level
- ✓ **Validation** - Multi-layer validation approach
- ✓ **Views** - Strongly-typed with tag helpers
- ✓ **CSS** - Organized with clear sections and comments

### Standards
- ✓ Follows C# naming conventions
- ✓ XML documentation comments
- ✓ Async/await for database operations
- ✓ Try-catch for error handling
- ✓ Configuration in appsettings.json

---

## 🐛 Troubleshooting

### Issue: Cannot connect to SQL Server
**Solution:**
- Verify SQL Server Express is running
- Check server name in connection string
- Ensure Windows Authentication is enabled
- Test connection in SQL Server Management Studio

### Issue: Database already exists error
**Solution:**
```bash
# Drop and recreate
dotnet ef database drop --force
dotnet ef database update
```

### Issue: Migrations not detected
**Solution:**
```bash
# Rebuild solution
dotnet build

# Check migrations folder
ls -la "Data/Migrations"
```

### Issue: HTTPS warning on startup
**Solution:**
- This is normal in development
- Application works fine on HTTP (port 5000)
- Use HTTPS (port 5001) in production

---

## 📚 Learning Resources

- **ASP.NET Core MVC**: https://learn.microsoft.com/aspnet/core/mvc/overview
- **Entity Framework Core**: https://learn.microsoft.com/ef/core/
- **SQL Server Express**: https://www.microsoft.com/sql-server/sql-server-express
- **C# Documentation**: https://learn.microsoft.com/dotnet/csharp/
- **Razor Syntax**: https://learn.microsoft.com/aspnet/core/mvc/views/razor

---

## 📋 Checklist for Interview Readiness

- ✅ **Clean Code** - Well-organized, follows conventions
- ✅ **Architecture** - Proper separation of concerns
- ✅ **Database Design** - Normalized schema with constraints
- ✅ **Validation** - Multi-level approach demonstrated
- ✅ **Error Handling** - Comprehensive try-catch blocks
- ✅ **User Experience** - Responsive, intuitive UI
- ✅ **Documentation** - Comments and guides provided
- ✅ **SQL Knowledge** - CHECK constraints, UNIQUE indexes
- ✅ **ORM Proficiency** - EF Core Fluent API usage
- ✅ **Best Practices** - DI, async/await, logging

---

## 🔄 Future Enhancements

Potential features to add:
- [ ] Search and filtering on student list
- [ ] Pagination for large datasets
- [ ] Export to PDF/Excel
- [ ] Student photo upload
- [ ] Admission status workflow
- [ ] Notifications/Email alerts
- [ ] Authentication and authorization
- [ ] Admin dashboard
- [ ] Reporting and analytics
- [ ] API layer for mobile apps

---

## 📄 License

This project is provided as-is for educational and interview purposes.

---

## 👨‍💻 Developer Information

**Developed By:** ASP.NET Core MVC Architect  
**Technology Stack:** .NET 10, Entity Framework Core, SQL Server Express  
**Architecture:** Clean MVC Pattern with 3-Layer Validation  
**Status:** ✅ Production-Ready, Interview-Ready  

---

## 📞 Support & Contact

For issues, questions, or improvements:
- Check [MIGRATION_INSTRUCTIONS.md](MIGRATION_INSTRUCTIONS.md) for database setup
- Review code comments and XML documentation
- Consult the troubleshooting section above
- Refer to official Microsoft documentation

---

**Last Updated:** February 26, 2026  
**Status:** ✅ Fully Functional  
**Database:** ✅ Created and Configured  
**Application:** ✅ Ready to Run
