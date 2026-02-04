using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        var ids = new List<int> { 1, 4, 5 };
        var dict = new Dictionary<int, int>
        {
            {1, 20000},
            {4, 40000},
            {5, 15000}
        };

        int total = 0;

        for (int i = 0; i < ids.Count; i++)
        {
            int id = ids[i];
            if (dict.ContainsKey(id))
            {
                total += dict[id];
            }
        }

        Console.WriteLine(total);
    }
}
