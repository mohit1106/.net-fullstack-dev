using System;
using System.Data;
using Microsoft.Data.SqlClient;
using EmployeeADOApp.Data;

namespace EmployeeADOApp.Services
{
    public class EmployeeService
    {
        // Part 1 — Employees by Department
        public static void GetEmployeesByDepartment(string dept)
        {
            using SqlConnection con = new SqlConnection(DbConfig.ConnectionString);
            using SqlCommand cmd = new SqlCommand("sp_GetEmployeesByDepartment", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Department", dept);

            con.Open();
            SqlDataReader dr = cmd.ExecuteReader();

            Console.WriteLine("\nEmployees:");
            while (dr.Read())
            {
                Console.WriteLine($"{dr["EmpId"]} | {dr["Name"]} | {dr["Department"]}");
            }
        }

        // Part 2 — OUTPUT parameter
        public static void GetDepartmentEmployeeCount(string dept)
        {
            using SqlConnection con = new SqlConnection(DbConfig.ConnectionString);
            using SqlCommand cmd = new SqlCommand("sp_GetDepartmentEmployeeCount", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Department", dept);

            SqlParameter output = new SqlParameter("@TotalEmployees", SqlDbType.Int);
            output.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(output);

            con.Open();
            cmd.ExecuteNonQuery();

            Console.WriteLine($"Total employees in {dept}: {output.Value}");
        }

        // Part 3 — Employee Orders
        public static void GetEmployeeOrders()
        {
            using SqlConnection con = new SqlConnection(DbConfig.ConnectionString);
            using SqlCommand cmd = new SqlCommand("sp_GetEmployeeOrders", con);

            cmd.CommandType = CommandType.StoredProcedure;

            con.Open();
            SqlDataReader dr = cmd.ExecuteReader();

            Console.WriteLine("\nEmployee Orders:");
            while (dr.Read())
            {
                Console.WriteLine(
                    $"{dr["Name"]} | {dr["Department"]} | {dr["OrderId"]} | {dr["OrderAmount"]} | {dr["OrderDate"]}"
                );
            }
        }

        // Part 4 — Duplicate Employees
        public static void GetDuplicateEmployees()
        {
            using SqlConnection con = new SqlConnection(DbConfig.ConnectionString);
            using SqlCommand cmd = new SqlCommand("sp_GetDuplicateEmployees", con);

            cmd.CommandType = CommandType.StoredProcedure;

            con.Open();
            SqlDataReader dr = cmd.ExecuteReader();

            Console.WriteLine("\nDuplicate Employees:");
            while (dr.Read())
            {
                Console.WriteLine(
                    $"{dr["EmpId"]} | {dr["Name"]} | {dr["Phone"]} | {dr["Email"]}"
                );
            }
        }
    }
}