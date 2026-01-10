using System;
using System.Collections.Generic;

namespace EcommerceAssessment
{
    public class Repository<T>
    {
        private List<T> items = new List<T>();

        public void Add(T item)
        {
            items.Add(item);
        }

        public List<T> GetAll()
        {
            return items;
        }
    }

    public class Order
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; }
        public double Amount { get; set; }

        public override string ToString()
        {
            return $"OrderId: {OrderId}, Customer: {CustomerName}, Amount: {Amount}";
        }
    }

    public delegate void OrderCallback(string message);

    public class OrderProcessor
    {
        public event Action<string> OrderProcessed;

        public void ProcessOrder(
            Order order,
            Func<double, double> taxCalculator,
            Func<double, double> discountCalculator,
            Predicate<Order> validator,  
            OrderCallback callback)
        {
            if (!validator(order))
            {
                callback("Order validation failed.");
                return;
            }

            double tax = taxCalculator(order.Amount);
            double discount = discountCalculator(order.Amount);
            order.Amount = order.Amount + tax - discount;

            callback($"Order {order.OrderId} processed successfully.");
            OrderProcessed?.Invoke($"Event: Order {order.OrderId} completed.");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Repository<Order> repository = new Repository<Order>();

            repository.Add(new Order { OrderId = 1, CustomerName = "Alice", Amount = 5000 });
            repository.Add(new Order { OrderId = 2, CustomerName = "Bob", Amount = 2000 });
            repository.Add(new Order { OrderId = 3, CustomerName = "Charlie", Amount = 8000 });

            Func<double, double> taxCalculator = amount => amount * 0.18;
            Func<double, double> discountCalculator = amount => amount * 0.05;
            Predicate<Order> validator = order => order.Amount >= 3000;

            OrderCallback callback = message => Console.WriteLine($"Callback: {message}");

            Action<string> logger = message => Console.WriteLine($"Logger: {message}");
            Action<string> notifier = message => Console.WriteLine($"Notifier: {message}");

            OrderProcessor processor = new OrderProcessor();
            processor.OrderProcessed += logger;
            processor.OrderProcessed += notifier;

            foreach (var order in repository.GetAll())
            {
                processor.ProcessOrder(order, taxCalculator, discountCalculator, validator, callback);
            }

            List<Order> processedOrders = repository.GetAll();
            processedOrders.Sort((o1, o2) => o2.Amount.CompareTo(o1.Amount));

            Console.WriteLine();
            Console.WriteLine("Sorted Orders (Descending Amount):");
            foreach (var order in processedOrders)
            {
                Console.WriteLine(order);
            }
        }
    }
}
