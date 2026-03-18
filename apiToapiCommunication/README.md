# API-to-API Communication Demo (.NET Microservices)

This workspace contains two ASP.NET Core Web API projects that communicate using synchronous HTTP request/response calls.

## Services

- `StudentService` (`http://localhost:5001`)
  - Owns student data and exposes student endpoints.
- `StudentInsightsService` (`http://localhost:5002`)
  - Calls `StudentService` over HTTP and returns derived/processed responses.

## Endpoints

### StudentService

- `GET /api/students`
  - Returns the full student list.
- `GET /api/students/{id}`
  - Returns one student by id.

### StudentInsightsService

- `GET /api/insights/summary`
  - Calls `StudentService`, then returns total students, average GPA, honor-roll count, and counts by major.
- `GET /api/insights/honor-roll`
  - Calls `StudentService`, then returns students with GPA >= 3.5.
- `GET /api/insights/top-student`
  - Calls `StudentService`, then returns the top GPA student.

## Run Locally

Open two terminals in the workspace root.

Terminal 1:

```powershell
dotnet run --project StudentService
```

Terminal 2:

```powershell
dotnet run --project StudentInsightsService
```

## Test the Communication

PowerShell examples:

```powershell
Invoke-RestMethod http://localhost:5001/api/students
Invoke-RestMethod http://localhost:5002/api/insights/summary
Invoke-RestMethod http://localhost:5002/api/insights/honor-roll
Invoke-RestMethod http://localhost:5002/api/insights/top-student
```

You can also open OpenAPI docs in development:

- `http://localhost:5001/openapi/v1.json`
- `http://localhost:5002/openapi/v1.json`
