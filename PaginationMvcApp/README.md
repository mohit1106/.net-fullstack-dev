# PaginationMvcApp - ASP.NET Core MVC Pagination Application

A complete ASP.NET Core MVC application demonstrating proper pagination implementation using Entity Framework Core with SQL Server Express.

## Project Structure

```
PaginationMvcApp/
├── Models/
│   └── Product.cs                 # Product model with Id, Name, Price
├── Data/
│   └── AppDbContext.cs            # Entity Framework Core DbContext
├── Controllers/
│   ├── HomeController.cs
│   └── ProductController.cs       # Handles pagination logic
├── Views/
│   ├── Product/
│   │   └── Index.cshtml           # Displays paginated products
│   └── Shared/
│       ├── _Layout.cshtml
│       └── Error.cshtml
├── Migrations/
│   ├── 20260305120000_InitialCreate.cs
│   └── AppDbContextModelSnapshot.cs
└── Program.cs                     # Application configuration
```

## Features

- **SQL Server Express Integration**: Uses SQL Server Express database at `XI\SQLEXPRESS`
- **Entity Framework Core**: Full EF Core setup with migrations
- **Pagination**: 
  - 5 products per page
  - Navigation using query parameter `?page={pageNumber}`
  - Previous/Next buttons
  - Page number links with smart range display
  - Current page highlighting
- **Seeded Data**: 30 sample products with IDs 1-30
- **Responsive UI**: Bootstrap-based styling for professional appearance

## Database Configuration

### Connection String
```
Server=XI\SQLEXPRESS
Database=PaginationDB
Trusted_Connection=True
TrustServerCertificate=True
```

This is configured in `appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=XI\\SQLEXPRESS;Database=PaginationDB;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

### Database Schema

#### Products Table
```sql
CREATE TABLE Products (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(MAX) NOT NULL,
    Price DECIMAL(18,2) NOT NULL
)
```

## Pagination Implementation

### Controller Logic (ProductController.cs)

The pagination uses `Skip()` and `Take()` for efficient database queries:

```csharp
int pageSize = 5;
var products = _context.Products
    .OrderBy(p => p.Id)
    .Skip((page - 1) * pageSize)  // Skip previous pages
    .Take(pageSize)                // Take only 5 products
    .ToList();
```

### View Pagination Display

- Current page indicator
- Previous button (disabled on page 1)
- Smart page range (shows current page ±2)
- Ellipsis (...) for hidden pages
- Next button (disabled on last page)
- All links use `?page={n}` query parameter

## How to Run

### Prerequisites
- .NET 10.0 SDK
- SQL Server Express running on `XI\SQLEXPRESS`
- Visual Studio Code with C# support (or any .NET IDE)

### Step 1: Restore Dependencies
```bash
dotnet restore
```

This installs all NuGet packages including:
- Microsoft.EntityFrameworkCore.SqlServer
- Microsoft.EntityFrameworkCore.Design

### Step 2: Build the Project
```bash
dotnet build
```

Compiles the solution and verifies no errors.

### Step 3: Run the Application
```bash
dotnet run
```

The application will:
1. Apply database migrations (creates `PaginationDB` if needed)
2. Seed 30 sample products
3. Start the server (typically at `https://localhost:5001`)

### Step 4: Access the Application
Open your browser and navigate to:
```
https://localhost:5001/Product/Index
```

Or click the "Products" link in the navigation bar.

## Pagination Usage

Navigate through products:
- **First page**: `https://localhost:5001/Product/Index?page=1`
- **Page 3**: `https://localhost:5001/Product/Index?page=3`
- **Last page**: `https://localhost:5001/Product/Index?page=6` (with 30 products, 5 per page)

Invalid page numbers are automatically corrected:
- Page 0 or negative → redirects to page 1
- Page > total pages → redirects to last page

## Complete Commands Quick Reference

```bash
# Restore NuGet packages
dotnet restore

# Build the solution
dotnet build

# Run the application
dotnet run

# Run with specific configuration
dotnet run --configuration Release

# Create a new migration (if you modify the model)
# dotnet ef migrations add <MigrationName>

# Apply pending migrations to database
# dotnet ef database update
```

## Troubleshooting

### Database Connection Issues
- Verify SQL Server Express is running on `XI\SQLEXPRESS`
- Check connection string in `appsettings.json`
- Ensure Windows Authentication is enabled for SQL Server

### Port Already in Use
If port 5001 is already in use:
```bash
dotnet run -- --urls "https://localhost:5002"
```

### Missing Migrations
Ensure migrations are applied:
```bash
dotnet ef database update
```

## Additional Notes

- The application uses implicit global usings and nullable reference types
- All HTTPS redirects are enabled
- CORS headers and static assets are configured
- Error handling uses the standard ASP.NET Core error page
- The layout includes Bootstrap for responsive design

## Example Products

The database seeds with 30 products:
- Product 1: $15.00
- Product 2: $20.00
- ...
- Product 30: $160.00

Each product increases in price by $5.00.

---

**Version**: 1.0  
**Framework**: ASP.NET Core 10.0  
**Database**: SQL Server Express  
**Last Updated**: March 5, 2026
