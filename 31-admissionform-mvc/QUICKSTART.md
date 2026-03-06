# 🚀 Quick Start Guide

## 2-Minute Setup

### Prerequisites
- ✓ .NET 10 SDK installed
- ✓ SQL Server Express (XI\SQLEXPRESS)
- ✓ VS Code or Visual Studio

### Run in 3 Commands

```bash
# 1. Navigate to project
cd "d:\Computer Eng\.net-fullstack-dev\31-admissionform-mvc"

# 2. Create database
dotnet ef database update

# 3. Run application
dotnet run
```

**Open:** http://localhost:5000/Students

---

## 📋 What to Test

### ✅ Create Student
1. Click "Add New Student"
2. Fill form:
   - FirstName: `John` (letters only)
   - LastName: `Doe`
   - Email: `john@example.com` (unique)
   - Phone: `9876543210` (exactly 10 digits)
   - DOB: Pick date (age >= 15)
   - Gender: Select from dropdown
   - Course: Select from dropdown
3. Click "Create Student"
4. ✓ See success message, record appears in list

### ✅ Validation Tests
Try invalid data:
- Empty fields → Required errors
- FirstName: `John123` → Letters only error
- Phone: `987654321` → 10 digits required
- DOB: Today's date → Age check fails
- Duplicate email → Uniqueness error

### ✅ Edit Student
1. Click ✏️ on any student
2. Modify data
3. Click "Update Student"
4. ✓ Changes saved, success message shown

### ✅ Soft Delete
1. Click 🗑️ on any student
2. Click "Yes, Delete This Student"
3. ✓ Record disappears from list
4. ✓ Data still in database (soft delete)

### ✅ View Details
1. Click 👁️ on any student
2. ✓ See read-only information
3. Can click Edit or Delete from here
4. Back button returns to list

---

## 📁 Key Files

| File | Purpose |
|------|---------|
| `Models/Student.cs` | Entity with validation |
| `Data/AdmissionDbContext.cs` | Database configuration |
| `Controllers/StudentsController.cs` | CRUD operations |
| `Views/Students/*.cshtml` | UI views |
| `wwwroot/css/site.css` | Styling |
| `appsettings.json` | DB connection string |

---

## 🔧 Common Commands

```bash
# Build project
dotnet build

# Run application
dotnet run

# Create new migration (after model change)
dotnet ef migrations add DescriptionHere -o "Data/Migrations"

# Apply migrations
dotnet ef database update

# Drop database
dotnet ef database drop --force

# View migration status
dotnet ef migrations list
```

---

## 🐛 Troubleshooting

### "Cannot connect to database"
```bash
dotnet ef database drop --force
dotnet ef database update
```

### "Students table already exists"
```bash
dotnet ef database drop --force
dotnet ef database update
```

### Port 5000/5001 already in use
```bash
dotnet run --urls "http://localhost:5050"
```

---

## 📚 Documentation Files

- **README.md** - Full project guide (450+ lines)
- **MIGRATION_INSTRUCTIONS.md** - Database setup (350+ lines)
- **ARCHITECTURE.md** - Technical design (400+ lines)
- **COMPLETION_SUMMARY.md** - Implementation checklist
- **This file** - Quick start

---

## ✨ Key Features

✓ Full CRUD operations  
✓ 3-layer validation (DataAnnotations, DbContext, SQL)  
✓ Soft delete (IsActive flag)  
✓ Email uniqueness constraint  
✓ Gender/Course dropdowns  
✓ Professional styling  
✓ Error handling & logging  
✓ Responsive design  

---

## 🎯 Endpoints

| Endpoint | Method | Purpose |
|----------|--------|---------|
| `/Students` | GET | List all active students |
| `/Students/Create` | GET | Show create form |
| `/Students/Create` | POST | Create new student |
| `/Students/Edit/{id}` | GET | Show edit form |
| `/Students/Edit/{id}` | POST | Update student |
| `/Students/Details/{id}` | GET | View student details |
| `/Students/Delete/{id}` | GET | Show delete confirmation |
| `/Students/Delete/{id}` | POST | Soft delete student |

---

## 💡 Interview Talking Points

**"This is a production-quality ASP.NET Core MVC application with:**
- Multi-layer validation (DataAnnotations, DbContext, SQL)
- Soft delete for compliance
- Entity Framework Core with migrations
- Clean separation of concerns
- Professional UI with responsive design
- Complete error handling and logging"

---

## 📝 Database Schema

```sql
CREATE TABLE [Students] (
    [StudentId] int NOT NULL IDENTITY,
    [FirstName] nvarchar(50) NOT NULL,
    [LastName] nvarchar(50) NOT NULL,
    [Email] nvarchar(100) NOT NULL,
    [Phone] nvarchar(15) NOT NULL,
    [DateOfBirth] date NOT NULL,
    [Gender] nvarchar(10) NOT NULL,
    [Course] nvarchar(100) NOT NULL,
    [AdmissionDate] datetime2 NOT NULL DEFAULT (GETDATE()),
    [IsActive] bit NOT NULL DEFAULT 1,
    
    CONSTRAINT [PK_Students] PRIMARY KEY ([StudentId]),
    CONSTRAINT [CK_Student_Gender] CHECK ([Gender] IN ('Male', 'Female', 'Other')),
    CONSTRAINT [CK_Student_Course] CHECK ([Course] IN ('CSE', 'ECE', 'MECH', 'CIVIL', 'EEE'))
);

CREATE UNIQUE INDEX [UX_Student_Email] ON [Students] ([Email]);
```

---

**Ready to Go! 🎉**

Questions? Check the documentation files for detailed information.
