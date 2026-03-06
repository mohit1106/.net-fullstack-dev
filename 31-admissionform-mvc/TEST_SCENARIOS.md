# Test Scenarios & Validation Cases

## Manual Testing Checklist

### ✅ Initial Setup

- [ ] Database `AdmissionDB` created in `XI\SQLEXPRESS`
- [ ] `Students` table exists with all 10 columns
- [ ] Application runs without errors: `dotnet run`
- [ ] Navigate to `http://localhost:5000/Students`
- [ ] Empty list shown with "No students found" message

---

## 📋 Test Case: Create Student - Valid Data

### Steps
1. Click "Add New Student" button
2. Fill form with valid data:
   - FirstName: `John`
   - LastName: `Smith`
   - Email: `john.smith@example.com`
   - Phone: `9876543210`
   - DateOfBirth: `01/01/2010` (14+ years ago)
   - Gender: `Male`
   - Course: `CSE`
3. Click "Create Student" button

### Expected Results
- ✓ Form submits successfully
- ✓ Success message: "Student record created successfully"
- ✓ Redirected to Student Index
- ✓ New record appears in table
- ✓ Record saved in database with:
  - Auto-generated StudentId
  - Current timestamp in AdmissionDate
  - IsActive = 1
  - All data as entered

### Validation Checks
- ✓ FirstName: `John` (valid - letters only)
- ✓ Email format valid (user@domain.com)
- ✓ Phone: exactly 10 digits
- ✓ Age: >= 15 years old
- ✓ Gender: valid from dropdown
- ✓ Course: valid from dropdown

---

## ❌ Test Case: Create Student - FirstName Validation

### Test Data - Invalid FirstName
| Test | Input | Expected Result |
|------|-------|-----------------|
| Empty | ` ` | "First name is required" |
| Numbers | `John123` | "First name must contain only letters" |
| Special chars | `John@` | "First name must contain only letters" |
| Too long | `John` × 20 chars | "First name cannot exceed 50 characters" |
| Valid | `John` | ✓ Accepted |

### Steps for "Numbers" Test
1. Go to Create form
2. Enter FirstName: `John123`
3. Enter valid data for other fields
4. Click "Create Student"

### Expected Result
- ✗ Form not submitted
- ✓ Error message: "First name must contain only letters"
- ✓ Form remains with entered data
- ✓ No record created

---

## ❌ Test Case: Create Student - LastName Validation

### Test Data - Invalid LastName
| Test | Input | Expected Result |
|------|-------|-----------------|
| Empty | ` ` | "Last name is required" |
| Numbers | `Smith123` | "Last name must contain only letters" |
| Special chars | `Smith@` | "Last name must contain only letters" |
| Too long | `Smith` × 20 chars | "Last name cannot exceed 50 characters" |
| Valid | `Smith` | ✓ Accepted |

---

## ❌ Test Case: Create Student - Email Validation

### Test Data - Invalid Email
| Test | Input | Expected Result |
|------|-------|-----------------|
| Empty | ` ` | "Email is required" |
| No @ | `johnexample.com` | "Please enter a valid email address" |
| No domain | `john@` | "Please enter a valid email address" |
| Invalid format | `john@domain` | May accept (basic validation) |
| Duplicate | (existing email) | "This email is already registered" |
| Valid | `john@example.com` | ✓ Accepted |

### Duplicate Email Test
1. Create first student with email: `duplicate@test.com`
2. Try to create second student with same email
3. Expected: Error "This email is already registered"
4. First record should exist, second should not

---

## ❌ Test Case: Create Student - Phone Validation

### Test Data - Invalid Phone
| Test | Input | Expected Result |
|------|-------|-----------------|
| Empty | ` ` | "Phone number is required" |
| 9 digits | `987654321` | "Phone number must be exactly 10 digits" |
| 11 digits | `98765432101` | "Phone number must be exactly 10 digits" |
| Letters | `98765432AB` | "Phone number must be exactly 10 digits" |
| With hyphens | `987-654-3210` | "Phone number must be exactly 10 digits" |
| With spaces | `987 654 3210` | "Phone number must be exactly 10 digits" |
| Valid | `9876543210` | ✓ Accepted |

---

## ❌ Test Case: Create Student - Date of Birth Validation

### Test Data - Invalid DOB
| Test | DateOfBirth | Age | Expected Result |
|------|-------------|-----|-----------------|
| Future | Tomorrow | -1 | "Please enter a valid date" |
| Today | Today | 0 | "Student must be at least 15 years old" |
| 14 years ago | 14 years | 14 | "Student must be at least 15 years old" |
| Exactly 15 years ago | 15 years | 15 | ✓ Accepted |
| 20 years ago | 20 years | 20 | ✓ Accepted |

### Example Test Cases
- **Invalid:** DOB = 2024-02-26 (today) → Age 0 → Rejected ✗
- **Invalid:** DOB = 2010-02-26 (14 years) → Age 14 → Rejected ✗
- **Valid:** DOB = 2009-02-26 (15 years) → Age 15 → Accepted ✓
- **Valid:** DOB = 2004-02-26 (20 years) → Age 20 → Accepted ✓

---

## ✅ Test Case: Create Student - Gender Dropdown

### Expected Behavior
1. Form displays Gender dropdown (not text input)
2. Default: "--Select Gender--" (empty selection)
3. Options: Male, Female, Other
4. Selecting nothing and submitting: "Gender is required"
5. Each option selectable: ✓ Male, ✓ Female, ✓ Other

### Test Data
| Test | Selection | Result |
|------|-----------|--------|
| No selection | None | "Gender is required" |
| Male | Male | ✓ Accepted |
| Female | Female | ✓ Accepted |
| Other | Other | ✓ Accepted |

---

## ✅ Test Case: Create Student - Course Dropdown

### Expected Behavior
1. Form displays Course dropdown (not text input)
2. Default: "--Select Course--" (empty selection)
3. Options: CSE, ECE, MECH, CIVIL, EEE
4. Selecting nothing and submitting: "Course is required"
5. Only predefined options available

### Test Data
| Test | Selection | Result |
|------|-----------|--------|
| No selection | None | "Course is required" |
| CSE | CSE | ✓ Accepted |
| ECE | ECE | ✓ Accepted |
| MECH | MECH | ✓ Accepted |
| CIVIL | CIVIL | ✓ Accepted |
| EEE | EEE | ✓ Accepted |
| Other | Invalid | Not selectable |

---

## ✅ Test Case: List Students (Index)

### Test - Empty List
1. Fresh database with no students
2. Navigate to `/Students`
3. Expected: "No students found" message displayed

### Test - With Students
1. Create 3 students with valid data
2. Navigate to `/Students`
3. Expected:
   - ✓ All 3 students displayed in table
   - ✓ Columns: FirstName, LastName, Email, Phone, Course, AdmissionDate
   - ✓ Action buttons for each row: View (👁️), Edit (✏️), Delete (🗑️)
   - ✓ Records sorted by FirstName, then LastName

### Test - Active Students Only Filter
1. Create student A (active)
2. Create student B (active)
3. Delete student A (soft delete)
4. Navigate to `/Students`
5. Expected:
   - ✓ Student B appears in list (active)
   - ✗ Student A does not appear (inactive)
   - Student A still exists in database with IsActive = 0

---

## ✅ Test Case: View Student Details

### Steps
1. Create a student with complete data
2. Click View button (👁️)
3. Verify details page

### Expected Results
- ✓ All fields displayed in read-only format
- ✓ FirstName, LastName, Email, Phone, DOB visible
- ✓ Gender and Course shown
- ✓ AdmissionDate displayed with full timestamp
- ✓ Status: Active/Inactive badge shown
- ✓ Email is clickable mailto link
- ✓ Course displayed as blue badge
- ✓ Navigation buttons: Edit, Delete, Back

---

## ✏️ Test Case: Edit Student - Valid Update

### Steps
1. Create student: `John Smith` / `john@test.com` / `9876543210` / CSE
2. Click Edit button
3. Change:
   - FirstName: `John` → `Jonathan`
   - Course: `CSE` → `ECE`
4. Click "Update Student"

### Expected Results
- ✓ Form pre-filled with current data
- ✓ Changes saved successfully
- ✓ Success message: "Student record updated successfully"
- ✓ Redirected to Index
- ✓ Updated data appears in table
- ✓ Database reflects changes

---

## ❌ Test Case: Edit Student - Email Uniqueness

### Steps
1. Create student A: `john@test.com`
2. Create student B: `jane@test.com`
3. Edit student B
4. Change email to: `john@test.com` (duplicate)
5. Click "Update Student"

### Expected Results
- ✗ Form does not submit
- ✓ Error message: "This email is already registered"
- ✓ Original data preserved in form
- ✓ Student B email remains `jane@test.com`

---

## ✏️ Test Case: Edit Student - IsActive Toggle

### Steps
1. Create active student
2. Edit student
3. Notice "Active Student" checkbox (checked)
4. Uncheck it
5. Click "Update Student"

### Expected Results
- ✓ Student marked as inactive (IsActive = 0)
- ✓ Success message shown
- ✓ Student disappears from list
- ✓ Data preserved in database

### Re-activate Test
1. Edit same student again
2. Check "Active Student" checkbox
3. Click "Update Student"
4. ✓ Student reappears in list
5. ✓ IsActive = 1 in database

---

## 🗑️ Test Case: Delete Student (Soft Delete)

### Steps
1. Create student with email: `delete@test.com`
2. Note student is in the list
3. Click Delete button (🗑️)
4. Verify delete confirmation page
5. Click "Yes, Delete This Student"

### Expected Results - UI
- ✓ Delete confirmation page shows correct student
- ✓ Warning message displayed
- ✓ Success message: "Student record deleted successfully"
- ✓ Redirected to Index
- ✗ Student not visible in list (deleted students filtered out)

### Expected Results - Database
- ✓ Record still exists in Students table
- ✓ StudentId, name, email, etc. unchanged
- ✓ IsActive = 0 (marked as deleted)
- ✓ Soft delete implemented correctly

### Verification Query
```sql
SELECT * FROM Students WHERE Email = 'delete@test.com'
-- Result: Record exists with IsActive = 0
```

---

## 🔄 Test Case: Complete Student Lifecycle

### Steps
1. **Create** → Student: `Alice Johnson` / `alice@test.com` / `9988776655` / ECE
2. Verify in list with IsActive = 1
3. **Edit** → Change LastName: `Johnson` → `Green`
4. Verify updated name in list
5. **View** → Click View button and verify all details
6. **Delete** → Click Delete and confirm
7. Verify not in list (but in database with IsActive = 0)

### Expected Outcomes
- ✓ Create: Record inserted with auto-generated StudentId
- ✓ Read: Record visible in active list; Details page correct
- ✓ Update: Changes saved and visible in list
- ✓ Delete: Soft delete sets IsActive = 0; record hidden from view

---

## 📊 Test Case: Multi-Student Scenarios

### Test: Multiple Students with Different Courses
1. Create 5 students:
   - John (CSE)
   - Jane (ECE)
   - Bob (MECH)
   - Alice (CIVIL)
   - Charlie (EEE)
2. Expected: All appear in list sorted by name

### Test: Gender Distribution
1. Create students with different genders:
   - TestA (Male)
   - TestB (Female)
   - TestC (Other)
2. Expected: All save correctly and appear in list

### Test: Age Boundary Cases
1. Create student with DOB = exactly 15 years ago today
   - Expected: ✓ Accepted
2. Create student with DOB = 15 years ago - 1 day
   - Expected: ✓ Accepted
3. Create student with DOB = 15 years - 1 day (14 years some months)
   - Expected: ✗ Rejected "Student must be at least 15 years old"

---

## 🔒 Test Case: Data Integrity at Database Level

### Test: Direct SQL INSERT with Invalid Gender
```sql
INSERT INTO Students (FirstName, LastName, Email, Phone, DateOfBirth, Gender, Course, IsActive)
VALUES ('Test', 'User', 'test@invalid.com', '9999999999', '2005-01-01', 'InvalidGender', 'CSE', 1)
```

### Expected Result
- ✗ Insert fails
- ✓ CHECK constraint violation error
- ✓ No record created

### Test: Direct SQL INSERT with Invalid Course
```sql
INSERT INTO Students (FirstName, LastName, Email, Phone, DateOfBirth, Gender, Course, IsActive)
VALUES ('Test', 'User', 'test@invalid.com', '9999999999', '2005-01-01', 'Male', 'InvalidCourse', 1)
```

### Expected Result
- ✗ Insert fails
- ✓ CHECK constraint violation error
- ✓ No record created

### Test: Duplicate Email at Database Level
```sql
INSERT INTO Students (FirstName, LastName, Email, Phone, DateOfBirth, Gender, Course, IsActive)
VALUES ('Duplicate', 'Test', 'exist@test.com', '9999999999', '2005-01-01', 'Male', 'CSE', 1)
```
(where `exist@test.com` already exists)

### Expected Result
- ✗ Insert fails
- ✓ UNIQUE constraint violation (UX_Student_Email index)
- ✗ Duplicate record not created

---

## ✅ Test Case: Form Validation Summary

### Test: Submit Empty Form
1. Go to Create
2. Click "Create Student" without entering anything
3. Expected: Validation summary shows all required field errors

### Count Check
- Should show approximately 7 required field errors:
  - FirstName required
  - LastName required
  - Email required
  - Phone required
  - DateOfBirth required
  - Gender required
  - Course required

---

## 📱 Test Case: Responsive Design

### Desktop (1920×1080)
- [ ] Form fields full width
- [ ] Table columns all visible
- [ ] Buttons properly spaced
- [ ] No horizontal scrolling

### Tablet (768×1024)
- [ ] Form fields respond to width
- [ ] Table headers visible
- [ ] Buttons wrap if needed
- [ ] Touch-friendly button size

### Mobile (375×667)
- [ ] Form fields stack vertically
- [ ] Buttons full width
- [ ] No horizontal overflow
- [ ] Readable font sizes

---

## 🎯 Test Case: Error Messages

### Verify Error Message Clarity

| Scenario | Error Message |
|----------|---------------|
| FirstName empty | "First name is required" |
| FirstName with numbers | "First name must contain only letters" |
| Email duplicate | "This email is already registered" |
| Phone not 10 digits | "Phone number must be exactly 10 digits" |
| Age < 15 | "Student must be at least 15 years old" |
| Gender not selected | "Gender is required" |
| Course not selected | "Course is required" |

- ✓ Error messages are clear and actionable
- ✓ Messages don't reference technical jargon
- ✓ Help user understand what's wrong
- ✓ Suggest how to fix the issue

---

## 🏁 Final Validation Checklist

- [ ] **Create:** All validations work, record saved correctly
- [ ] **Read:** List shows only active students, details page complete
- [ ] **Update:** Changes saved, email unique enforcement works
- [ ] **Delete:** Soft delete implemented, data preserved
- [ ] **Validation:** 3 levels enforced (UI, server, database)
- [ ] **UI:** Professional styling, responsive design
- [ ] **Error Handling:** Clear messages, form preserved
- [ ] **Database:** AdmissionDB created, constraints working
- [ ] **Performance:** Pages load quickly, no timeout errors
- [ ] **Documentation:** All setup guides complete

---

**Test Status:** Ready for comprehensive manual testing  
**Expected Duration:** ~30-45 minutes for complete test suite  
**Success Criteria:** All test cases pass with expected results
