using System;
using System.Data.SqlClient;

class Program
{
    static void Main()
    {
        string connectionString = "Data Source=.;Initial Catalog=BankDB;Integrated Security=True";

        int senderId = 1;
        int receiverId = 2;
        decimal amount = 1000;

        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            SqlTransaction tx = conn.BeginTransaction();

            try
            {
                SqlCommand deductCmd = new SqlCommand(
                    "UPDATE Accounts SET Balance = Balance - @Amount WHERE Id = @SenderId",
                    conn, tx);
                deductCmd.Parameters.AddWithValue("@Amount", amount);
                deductCmd.Parameters.AddWithValue("@SenderId", senderId);
                deductCmd.ExecuteNonQuery();

                SqlCommand addCmd = new SqlCommand(
                    "UPDATE Accounts SET Balance = Balance + @Amount WHERE Id = @ReceiverId",
                    conn, tx);
                addCmd.Parameters.AddWithValue("@Amount", amount);
                addCmd.Parameters.AddWithValue("@ReceiverId", receiverId);
                addCmd.ExecuteNonQuery();

                tx.Commit();
                Console.WriteLine("Transaction Successful");
            }
            catch
            {
                tx.Rollback();
                Console.WriteLine("Transaction Rolled Back");
            }
        }
    }
}