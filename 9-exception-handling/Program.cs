// using System;
// using System.IO;

// class InsufficientBalanceException : Exception
// {
//     public InsufficientBalanceException(string message) : base(message) { }
// }

// class BankAccount
// {
//     public decimal Balance { get; private set; } = 5000;

//     public void Withdraw(decimal amount)
//     {
//         if (amount <= 0)
//             throw new ArgumentException("Withdrawal amount must be greater than 0");

//         if (amount > Balance)
//             throw new InsufficientBalanceException("Insufficient balance");

//         Balance -= amount;
//     }
// }

// class Program
// {
//     static void Main()
//     {
//         BankAccount account = new BankAccount();

//         try
//         {
//             account.Withdraw(6000);
//             Console.WriteLine("Withdrawal successful. Remaining balance: " + account.Balance);
//         }
//         catch (InsufficientBalanceException ex)
//         {
//             LogException(ex);
//             Console.WriteLine(ex.Message);
//         }
//         catch (Exception ex)
//         {
//             LogException(ex);
//             Console.WriteLine("An unexpected error occurred");
//         }
//         finally
//         {
//             Console.WriteLine("Transaction attempt completed.");
//         }
//     }

//     static void LogException(Exception ex)
//     {
//         File.AppendAllText(
//             "error.log",
//             $"{DateTime.Now} | {ex.GetType().Name} | {ex.Message}{Environment.NewLine}"
//         );
//     }
// }




using System;
using System.IO;
class Program {

    public static void Main(){
        try
        {
            try
            {
                File.ReadAllText("transactions.txt");
            }
            catch (IOException ioEx)
            {
                throw new ApplicationException(
                    "Unable to load transaction data",
                    ioEx
                );
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Message: " + ex.Message);
            Console.WriteLine("Root Cause: " + ex.InnerException.Message);
        }

    }
}