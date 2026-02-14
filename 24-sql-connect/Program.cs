using System;
using Microsoft.Data.SqlClient;  // dotnet add package Microsoft.Data.SqlClient
using System.Data;
using System.Reflection.PortableExecutable;

class Program
{
    static void Main()
    {
        // 1. Define the connection string
        // Replace [ServerName] and [DatabaseName] with your actual SQL Server details.
        // Use "Trusted_Connection=True" for Windows Authentication (integrated security)
        // or "User Id=myUsername;Password=myPassword;" for SQL Server Authentication.
        string connectionString =
    "Data Source=localhost\\SQLEXPRESS;" +
    "Initial Catalog=lpu;" +
    "Integrated Security=True;" +
    "Encrypt=True;TrustServerCertificate=True;";

        // 2. Create a SqlConnection object within a 'using' statement
        // The 'using' statement ensures the connection is automatically closed and disposed
        // even if errors occur.
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            try
            {
                // 3. Open the connection
                connection.Open();
                Console.WriteLine("Connection successful!");

                // 4. Define and execute a SQL command
                string query = "SELECT Name,Department from CollegeMaster1";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                    // Use parameters to prevent SQL injection
                    SqlCommandBuilder sqlee = new SqlCommandBuilder(dataAdapter);

                  



                    


                    // Use SqlDataReader to read data from the database
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        Console.WriteLine("\nReading data...");
                        while (reader.Read())
                        {
                            // Access data by column name or index
                            Console.WriteLine($"{reader["Name"]} {reader["Department"]}");
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                // Handle any errors that may occur during the connection or query
                Console.WriteLine($"Error: {ex.Message}");
            }
            // The connection is implicitly closed when the 'using' block ends (even in case of error)
            Console.WriteLine("Connection closed.");
            Hyy();
        }
    }

    private static void Hyy()
    {
    }
}