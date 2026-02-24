using System;
using System.Collections.Generic;
using System.Data.SqlClient;

class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}

class Program
{
    static void Main()
    {
        List<Product> products = new List<Product>();

        string connectionString = "Data Source=.;Initial Catalog=ProductDB;Integrated Security=True";

        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string query = "SELECT Id, Name, Price FROM Product";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        products.Add(new Product
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Price = reader.GetDecimal(2)
                        });
                    }
                }
            }
        }

        foreach (var p in products)
        {
            Console.WriteLine($"{p.Id} {p.Name} {p.Price}");
        }
    }
}