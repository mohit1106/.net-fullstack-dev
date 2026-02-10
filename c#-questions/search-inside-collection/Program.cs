using System;
using System.Collections.Generic;
using System.Linq;

public class Program
{
    public static SortedDictionary<string, long> itemDetails =
        new SortedDictionary<string, long>()
        {
            { "Pen", 150 },
            { "Notebook", 300 },
            { "Pencil", 100 },
            { "Eraser", 50 }
        };

    public static SortedDictionary<string, long> FindItemDetails(long soldCount)
    {
        SortedDictionary<string, long> result = new SortedDictionary<string, long>();

        foreach (var kvp in itemDetails)
        {
            if (kvp.Value == soldCount)
            {
                result.Add(kvp.Key, kvp.Value);
            }
        }

        return result;
    }

    public static List<string> FindMinandMaxSoldItems()
    {
        List<string> result = new List<string>();

        var minItem = itemDetails.Aggregate((a, b) => a.Value < b.Value ? a : b);
        var maxItem = itemDetails.Aggregate((a, b) => a.Value > b.Value ? a : b);

        result.Add(minItem.Key);
        result.Add(maxItem.Key);

        return result;
    }

    public static Dictionary<string, long> SortByCount()
    {
        Dictionary<string, long> sortedResult = new Dictionary<string, long>();
        List<KeyValuePair<string, long>> itemsList = itemDetails.ToList();
        itemsList.Sort((a, b) => a.Value.CompareTo(b.Value));
        foreach (var item in itemsList)
        {
            sortedResult.Add(item.Key, item.Value);
        }
        return sortedResult;
    }


    static void Main(string[] args)
    {
        long soldCount = 100;

        SortedDictionary<string, long> foundItems = FindItemDetails(soldCount);

        if (foundItems.Count == 0)
        {
            Console.WriteLine("Invalid sold count");
        }
        else
        {
            Console.WriteLine("Item Details:");
            foreach (var item in foundItems)
            {
                Console.WriteLine(item.Key + " : " + item.Value);
            }
        }

        List<string> minMaxItems = FindMinandMaxSoldItems();
        Console.WriteLine("Minimum Sold Item: " + minMaxItems[0]);
        Console.WriteLine("Maximum Sold Item: " + minMaxItems[1]);

        Dictionary<string, long> sortedItems = SortByCount();
        Console.WriteLine("Items Sorted by Sold Count:");
        foreach (var item in sortedItems)
        {
            Console.WriteLine(item.Key + " : " + item.Value);
        }
    }
}
