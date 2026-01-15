using System;

class Program
{
    static void Main()
    {
        int choice;

        do
        {
            Console.WriteLine("\n--- Finance System ---");
            Console.WriteLine("1. Check Loan Eligibility");
            Console.WriteLine("2. Calculate Tax");
            Console.WriteLine("3. Enter Transactions");
            Console.WriteLine("4. Exit");
            Console.Write("Enter your choice: ");

            choice = Convert.ToInt32(Console.ReadLine());
            switch (choice)
            {
                case 1:
                    CheckLoanEligibility();
                    break;
                case 2:
                    CalculateTax();
                    break;
                case 3:
                    EnterTransactions();
                    break;
                case 4:
                    Console.WriteLine("Exiting program...");
                    break;
                default:
                    Console.WriteLine("Invalid choice. Try again.");
                    break;
            }
        } while (choice != 4);
    }

    static void CheckLoanEligibility()
    {
        Console.Write("Enter age: ");
        int age = Convert.ToInt32(Console.ReadLine());
        Console.Write("Enter monthly income: ");
        double income = Convert.ToDouble(Console.ReadLine());

        if (age >= 21 && income >= 30000) Console.WriteLine("Loan Eligible");
        else Console.WriteLine("Not Eligible for Loan");
    }

    static void CalculateTax()
    {
        Console.Write("Enter annual income: ");
        double income = Convert.ToDouble(Console.ReadLine());
        double tax = 0;

        if (income <= 250000) tax = 0;
        else if (income <= 500000) tax = (income - 250000) * 0.05;
        else if (income <= 1000000) tax = (250000 * 0.05) + (income - 500000) * 0.20;
        else tax = (250000 * 0.05) + (500000 * 0.20) + (income - 1000000) * 0.30;

        Console.WriteLine("Tax payable: " + tax);
    }

    static void EnterTransactions()
    {
        int count = 0;
        double total = 0;

        Console.WriteLine("Enter 5 transactions:");
        while (count < 5)
        {
            Console.Write("Transaction " + (count + 1) + ": ");
            double amount = Convert.ToDouble(Console.ReadLine());
            if (amount < 0)
            {
                Console.WriteLine("Invalid transaction. Skipped.");
                continue;
            }
            total += amount;
            count++;
        }
        Console.WriteLine("Total of valid transactions: ₹" + total);
    }
}
