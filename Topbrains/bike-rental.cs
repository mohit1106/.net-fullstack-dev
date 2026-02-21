using System;
using System.Collections.Generic;

class Bike
{
    public string Model { get; set; }
    public int PricePerDay { get; set; }
    public string Brand { get; set; }
}

class BikeUtility
{
    public void AddBikeDetails(string model, string brand, int pricePerDay)
    {
        int key = Program.bikeDetails.Count + 1;
        Program.bikeDetails.Add(key, new Bike
        {
            Model = model,
            Brand = brand,
            PricePerDay = pricePerDay
        });
    }

    public SortedDictionary<string, List<Bike>> GroupBikesByBrand()
    {
        SortedDictionary<string, List<Bike>> result = new SortedDictionary<string, List<Bike>>();

        foreach (var item in Program.bikeDetails.Values)
        {
            if (!result.ContainsKey(item.Brand))
                result[item.Brand] = new List<Bike>();

            result[item.Brand].Add(item);
        }

        return result;
    }
}

class Program
{
    public static SortedDictionary<int, Bike> bikeDetails = new SortedDictionary<int, Bike>();

    static void Main()
    {
        BikeUtility utility = new BikeUtility();

        while (true)
        {
            Console.WriteLine("1. Add Bike Details");
            Console.WriteLine("2. Group Bikes By Brand");
            Console.WriteLine("3. Exit");
            Console.WriteLine();
            Console.Write("Enter your choice ");

            int choice = int.Parse(Console.ReadLine());

            if (choice == 1)
            {
                Console.WriteLine();
                Console.Write("Enter the model: ");
                string model = Console.ReadLine();

                Console.Write("Enter the brand: ");
                string brand = Console.ReadLine();

                Console.Write("Enter the price per day: ");
                int price = int.Parse(Console.ReadLine());

                utility.AddBikeDetails(model, brand, price);

                Console.WriteLine();
                Console.WriteLine("Bike details added successfully");
                Console.WriteLine();
            }
            else if (choice == 2)
            {
                Console.WriteLine();
                var grouped = utility.GroupBikesByBrand();

                foreach (var brand in grouped)
                {
                    foreach (var bike in brand.Value)
                    {
                        Console.WriteLine(brand.Key + " " + bike.Model);
                    }
                }

                Console.WriteLine();
            }
            else if (choice == 3)
            {
                break;
            }
        }
    }
}
