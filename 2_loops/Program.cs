using System;
using System.Collections.Generic;

public class Order
{
    public int OrderId { get; set; }
    public string CustomerName { get; set; }
    public string Item { get; set; }

    public Stack<Order> AddOrderDetails(int orderId, string customerName, string item)
    {
        Order o = new Order
        {
            OrderId = orderId,
            CustomerName = customerName,
            Item = item
        };

        Program.OrderStack.Push(o);
        return Program.OrderStack;
    }

    public string GetOrderDetails()
    {
        if (Program.OrderStack.Count == 0)
            return "No Orders";

        Order o = Program.OrderStack.Peek();
        return o.OrderId + " " + o.CustomerName + " " + o.Item;
    }

    public Stack<Order> RemoveOrderDetails()
    {
        if (Program.OrderStack.Count > 0)
        {
            Program.OrderStack.Pop();
        }

        return Program.OrderStack;
    }
}

public class Program
{
    public static Stack<Order> OrderStack { get; set; } = new Stack<Order>();

    public static void Main()
    {
        Order order = new Order();

        int n = int.Parse(Console.ReadLine());

        for (int i = 0; i < n; i++)
        {
            int orderId = int.Parse(Console.ReadLine());
            string customerName = Console.ReadLine();
            string item = Console.ReadLine();

            order.AddOrderDetails(orderId, customerName, item);
        }

        Console.WriteLine(order.GetOrderDetails());
        Stack<Order> remaining = order.RemoveOrderDetails();
        foreach (Order o in remaining)
        {
            Console.WriteLine(o.OrderId + " " + o.CustomerName + " " + o.Item);
        }
    }
}
