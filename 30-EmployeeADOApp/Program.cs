using System;
using EmployeeADOApp.Services;

namespace EmployeeADOApp
{
    class Program
    {
        static void Main()
        {
            Console.Write("Enter Department: ");
            string dept = Console.ReadLine();

            EmployeeService.GetEmployeesByDepartment(dept);
            EmployeeService.GetDepartmentEmployeeCount(dept);
            EmployeeService.GetEmployeeOrders();
            EmployeeService.GetDuplicateEmployees();

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}


// using System;
// using Microsoft.Data.SqlClient;
// using EmployeeADOApp.Data;

// class Program
// {
//     static void Main()
//     {
//         try
//         {
//             using SqlConnection con = new SqlConnection(DbConfig.ConnectionString);
//             con.Open();
//             Console.WriteLine("SQL CONNECTION SUCCESS ✅");
//         }
//         catch (Exception ex)
//         {
//             Console.WriteLine("FAILED ❌");
//             Console.WriteLine(ex.Message);
//         }
//     }
// }