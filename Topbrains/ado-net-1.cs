using System;
using System.Data.SqlClient;

class Program
{
    static void Main()
    {
        Console.Write("Enter Id: ");
        int id = int.Parse(Console.ReadLine());

        Console.Write("Enter Name: ");
        string name = Console.ReadLine();

        Console.Write("Enter Marks: ");
        int marks = int.Parse(Console.ReadLine());

        string connectionString = "Data Source=.;Initial Catalog=StudentDB;Integrated Security=True";

        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string query = "INSERT INTO Student (Id, Name, Marks) VALUES (@Id, @Name, @Marks)";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@Marks", marks);

                conn.Open();
                cmd.ExecuteNonQuery();
                Console.WriteLine("Inserted Successfully");
            }
        }
    }
}