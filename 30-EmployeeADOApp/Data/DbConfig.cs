using Microsoft.Data.SqlClient;

namespace EmployeeADOApp.Data
{
    public static class DbConfig
    {
        public static string ConnectionString =
            "Server=XI\\SQLEXPRESS;Database=EmployeeDB;Integrated Security=True;TrustServerCertificate=True";
    }
}