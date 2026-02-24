using System;
using System.Data;
using System.Data.SqlClient;

class Program
{
    static void Main()
    {
        string connectionString = "Data Source=.;Initial Catalog=EmployeeDB;Integrated Security=True";

        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            SqlDataAdapter adapter = new SqlDataAdapter("SELECT Id, Name, Salary FROM Employee", conn);

            SqlCommandBuilder builder = new SqlCommandBuilder(adapter);

            DataSet ds = new DataSet();

            adapter.Fill(ds, "Employee");

            DataTable table = ds.Tables["Employee"];

            if (table.Rows.Count > 0)
            {
                table.Rows[0]["Salary"] = Convert.ToDecimal(table.Rows[0]["Salary"]) + 5000;
            }

            adapter.Update(ds, "Employee");

            Console.WriteLine("Database Updated Successfully");
        }
    }
}