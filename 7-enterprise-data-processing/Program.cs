using System;
using System.Collections;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        Console.WriteLine("=== Enterprise Data Processing & Control System ===\n");

        Console.WriteLine("TASK 1 - DYNAMIC PRODUCT PRICE");
        int nProducts = ReadPositiveInt("Enter number of products: ");
        int[] prices = new int[nProducts];

        for (int i = 0; i < nProducts; i++)
        {
            prices[i] = ReadPositiveInt($"Enter positive price for product[{i}]: ");
        }

        double avgDouble = CalculateAverage(prices);
        int avgRounded = (int)Math.Round(avgDouble);

        Console.WriteLine($"\nAverage price (exact): {avgDouble:F2}");
        Console.WriteLine($"Average price (rounded to int for array fill): {avgRounded}");

        Array.Sort(prices);
        Console.WriteLine("\nSorted prices:");
        PrintIndexedArray(prices);

        for (int i = 0; i < prices.Length; i++)
        {
            if (prices[i] < avgDouble)
                prices[i] = 0;
        }
        Console.WriteLine("\nAfter replacing prices below average with 0:");
        PrintIndexedArray(prices);

        Array.Resize(ref prices, prices.Length + 5);
        for (int i = prices.Length - 5; i < prices.Length; i++)
            prices[i] = avgRounded;

        Console.WriteLine("\nFinal array after resizing (+5) and filling with average:");
        PrintIndexedArray(prices);
        Console.WriteLine(new string('-', 60) + "\n");














        Console.WriteLine("TASK 2 - BRANCH SALES ANALYSIS");
        int nBranches = ReadPositiveInt("Enter number of branches: ");
        int nMonths = ReadPositiveInt("Enter number of months: ");
        int[,] sales2D = new int[nBranches, nMonths];

        for (int b = 0; b < nBranches; b++)
        {
            Console.WriteLine($"--- Branch {b} input ---");
            for (int m = 0; m < nMonths; m++)
            {
                sales2D[b, m] = ReadNonNegativeInt($"Enter sales for branch {b}, month {m}: ");
            }
        }

        int[] branchTotals = new int[nBranches];
        int globalHighest = int.MinValue;
        for (int b = 0; b < nBranches; b++)
        {
            int total = 0;
            for (int m = 0; m < nMonths; m++)
            {
                int val = sales2D[b, m];
                total += val;
                if (val > globalHighest) globalHighest = val;
            }
            branchTotals[b] = total;
        }

        Console.WriteLine("\nBranch totals:");
        for (int b = 0; b < nBranches; b++)
            Console.WriteLine($"Branch {b} total sales = {branchTotals[b]}");

        if (globalHighest == int.MinValue) globalHighest = 0;
        Console.WriteLine($"Highest monthly sale across all branches = {globalHighest}");
        Console.WriteLine(new string('-', 60) + "\n");


























        Console.WriteLine("TASK 3 - PERFORMANCE-BASED DATA EXTRACTION (JAGGED ARRAY)");
        int[][] jagged = new int[nBranches][];
        for (int b = 0; b < nBranches; b++)
        {
            int count = 0;
            for (int m = 0; m < nMonths; m++)
            {
                if (sales2D[b, m] >= avgRounded) count++;
            }

            jagged[b] = new int[count];
            int idx = 0;
            for (int m = 0; m < nMonths; m++)
            {
                if (sales2D[b, m] >= avgRounded)
                {
                    jagged[b][idx++] = sales2D[b, m];
                }
            }
        }

        Console.WriteLine($"\nJagged array (per branch) - only sales >= product average ({avgRounded}):");
        for (int b = 0; b < nBranches; b++)
        {
            Console.Write($"Branch {b}: ");
            if (jagged[b].Length == 0) Console.WriteLine("(no qualifying values)");
            else
            {
                for (int k = 0; k < jagged[b].Length; k++)
                {
                    Console.Write(jagged[b][k] + (k < jagged[b].Length - 1 ? ", " : ""));
                }
                Console.WriteLine();
            }
        }
        Console.WriteLine(new string('-', 60) + "\n");



















        Console.WriteLine("TASK 4 - CUSTOMER TRANSACTION CLEANING (List & HashSet)");
        int nCustTx = ReadNonNegativeInt("Enter number of customer transactions: ");
        List<int> custList = new List<int>();
        for (int i = 0; i < nCustTx; i++)
        {
            int id = ReadNonNegativeInt($"Enter customer ID [{i}]: ");
            custList.Add(id);
        }

        Console.WriteLine($"\nOriginal customer transaction count = {custList.Count}");
        HashSet<int> custSet = new HashSet<int>(custList);
        List<int> cleanedList = new List<int>(custSet);
        Console.WriteLine("Cleaned customer list (duplicates removed):");
        for (int i = 0; i < cleanedList.Count; i++)
            Console.WriteLine($"[{i}] = {cleanedList[i]}");

        int duplicatesRemoved = custList.Count - cleanedList.Count;
        Console.WriteLine($"Number of duplicate entries removed = {duplicatesRemoved}");
        Console.WriteLine(new string('-', 60) + "\n");
























        Console.WriteLine("TASK 5 - FINANCIAL TRANSACTION FILTERING (Dictionary & SortedList)");
        int nTx = ReadNonNegativeInt("Enter number of financial transactions: ");
        Dictionary<int, double> txDict = new Dictionary<int, double>();

        for (int i = 0; i < nTx; i++)
        {
            int txId;
            while (true)
            {
                txId = ReadNonNegativeInt($"Enter transaction ID [{i}]: ");
                if (!txDict.ContainsKey(txId)) break;
                Console.WriteLine("Duplicate transaction ID detected. Transaction IDs must be unique. Enter again.");
            }
            double amount = ReadNonNegativeDouble($"Enter amount for transaction {txId}: ");
            txDict[txId] = amount;
        }

        SortedList<int, double> highValue = new SortedList<int, double>();
        foreach (KeyValuePair<int, double> kv in txDict)
        {
            if (kv.Value >= avgRounded)
                highValue.Add(kv.Key, kv.Value);
        }

        Console.WriteLine("\nSorted high-value transactions (amount >= product average):");
        if (highValue.Count == 0) Console.WriteLine("(none)");
        else
        {
            foreach (KeyValuePair<int, double> kv in highValue)
                Console.WriteLine($"Transaction ID = {kv.Key}, Amount = {kv.Value:F2}");
        }
        Console.WriteLine(new string('-', 60) + "\n");
























        // TASK 6 - Process Flow Management (Queue & Stack)
        Console.WriteLine("TASK 6 - PROCESS FLOW MANAGEMENT (Queue & Stack)");
        int nOps = ReadNonNegativeInt("Enter number of operations: ");
        Queue<string> processingQueue = new Queue<string>();
        Stack<string> undoStack = new Stack<string>();

        for (int i = 0; i < nOps; i++)
        {
            Console.Write($"Enter operation [{i}] (single-line): ");
            string op = Console.ReadLine() ?? string.Empty;
            processingQueue.Enqueue(op);
            undoStack.Push(op);
        }

        Console.WriteLine("\nProcessing queue (FIFO):");
        List<string> processed = new List<string>();
        while (processingQueue.Count > 0)
        {
            string current = processingQueue.Dequeue();
            processed.Add(current);
            Console.WriteLine($"Processed: {current}");
        }

        Console.WriteLine("\nUndo last two operations using Stack (LIFO):");
        List<string> undone = new List<string>();
        for (int i = 0; i < 2; i++)
        {
            if (undoStack.Count > 0)
            {
                string u = undoStack.Pop();
                undone.Add(u);
                Console.WriteLine($"Undone: {u}");
            }
            else
            {
                Console.WriteLine("No more operations to undo.");
            }
        }

        Console.WriteLine(new string('-', 60) + "\n");







































        Console.WriteLine("TASK 7 - LEGACY DATA RISK DEMONSTRATION (Hashtable & ArrayList)");
        int nUsers = ReadNonNegativeInt("Enter number of users to store (username-role pairs): ");
        Hashtable ht = new Hashtable();
        ArrayList al = new ArrayList();

        for (int i = 0; i < nUsers; i++)
        {
            Console.Write($"Enter username [{i}]: ");
            string username = Console.ReadLine() ?? string.Empty;
            Console.Write($"Enter role for {username}: ");
            string role = Console.ReadLine() ?? string.Empty;

            // Hashtable insertion (keys unique)
            if (!ht.ContainsKey(username))
                ht[username] = role;
            else
                Console.WriteLine($"Hashtable: username '{username}' already exists - skipping duplicate key insertion.");

            // ArrayList: mixed storage
            al.Add(username); // string
            al.Add(role);     // string
            // demonstrate mixing another type intentionally (to show risk)
            if (i % 2 == 0) al.Add(i); // mixed int occasionally
        }

        Console.WriteLine("\nHashtable contents (username -> role):");
        foreach (DictionaryEntry de in ht)
        {
            Console.WriteLine($"{de.Key} -> {de.Value}");
        }

        Console.WriteLine("\nArrayList contents (mixed types, stored as sequence):");
        for (int i = 0; i < al.Count; i++)
        {
            object item = al[i];
            Console.WriteLine($"Index {i}: Value = {item} (Type = {item?.GetType().Name ?? "null"})");
        }

        Console.WriteLine("\nDemonstration of risk: ArrayList contains mixed types so retrieving expecting one structure can fail or require casting checks.");
        Console.WriteLine("E.g., trying to cast every item to string will throw or produce invalid data for integer elements.");

        Console.WriteLine("\n--- Program completed ---");
        Console.WriteLine("Press Enter to exit.");
        Console.ReadLine();
    }


























    static int ReadPositiveInt(string prompt)
    {
        int val;
        while (true)
        {
            Console.Write(prompt);
            string? s = Console.ReadLine();
            if (int.TryParse(s, out val) && val > 0) return val;
            Console.WriteLine("Invalid input. Enter a positive integer (> 0).");
        }
    }

    static int ReadNonNegativeInt(string prompt)
    {
        int val;
        while (true)
        {
            Console.Write(prompt);
            string? s = Console.ReadLine();
            if (int.TryParse(s, out val) && val >= 0) return val;
            Console.WriteLine("Invalid input. Enter a non-negative integer (>= 0).");
        }
    }

    static double ReadNonNegativeDouble(string prompt)
    {
        double val;
        while (true)
        {
            Console.Write(prompt);
            string? s = Console.ReadLine();
            if (double.TryParse(s, out val) && val >= 0.0) return val;
            Console.WriteLine("Invalid input. Enter a non-negative number (>= 0).");
        }
    }

    static double CalculateAverage(int[] arr)
    {
        if (arr.Length == 0) return 0.0;
        long sum = 0;
        foreach (int v in arr) sum += v;
        return (double)sum / arr.Length;
    }

    static void PrintIndexedArray(int[] arr)
    {
        for (int i = 0; i < arr.Length; i++)
            Console.WriteLine($"Index [{i}] = {arr[i]}");
    }
}
