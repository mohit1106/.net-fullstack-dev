using System;

namespace QuickMartTraders
{
    public class SaleTransaction
    {
        public string InvoiceNo { get; set; }
        public string CustomerName { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public decimal PurchaseAmount { get; set; }
        public decimal SellingAmount { get; set; }
        public string ProfitOrLossStatus { get; set; }
        public decimal ProfitOrLossAmount { get; set; }
        public decimal ProfitMarginPercent { get; set; }

        public void Calculate()
        {
            if (SellingAmount > PurchaseAmount)
            {
                ProfitOrLossStatus = "PROFIT";
                ProfitOrLossAmount = SellingAmount - PurchaseAmount;
            }
            else if (SellingAmount < PurchaseAmount)
            {
                ProfitOrLossStatus = "LOSS";
                ProfitOrLossAmount = PurchaseAmount - SellingAmount;
            }
            else
            {
                ProfitOrLossStatus = "BREAK-EVEN";
                ProfitOrLossAmount = 0;
            }

            ProfitMarginPercent = PurchaseAmount > 0
                ? (ProfitOrLossAmount / PurchaseAmount) * 100
                : 0;
        }
    }

    public class TransactionService
    {
        public static SaleTransaction LastTransaction;
        public static bool HasLastTransaction;

        public static void CreateTransaction()
        {
            string invoiceNo;
            do
            {
                Console.Write("Enter Invoice No: ");
                invoiceNo = Console.ReadLine();
            } while (string.IsNullOrWhiteSpace(invoiceNo));

            Console.Write("Enter Customer Name: ");
            string customerName = Console.ReadLine();

            Console.Write("Enter Item Name: ");
            string itemName = Console.ReadLine();

            int quantity;
            while (true)
            {
                Console.Write("Enter Quantity: ");
                if (int.TryParse(Console.ReadLine(), out quantity) && quantity > 0)
                    break;
            }

            decimal purchaseAmount;
            while (true)
            {
                Console.Write("Enter Purchase Amount (total): ");
                if (decimal.TryParse(Console.ReadLine(), out purchaseAmount) && purchaseAmount > 0)
                    break;
            }

            decimal sellingAmount;
            while (true)
            {
                Console.Write("Enter Selling Amount (total): ");
                if (decimal.TryParse(Console.ReadLine(), out sellingAmount) && sellingAmount >= 0)
                    break;
            }

            SaleTransaction transaction = new SaleTransaction
            {
                InvoiceNo = invoiceNo,
                CustomerName = customerName,
                ItemName = itemName,
                Quantity = quantity,
                PurchaseAmount = purchaseAmount,
                SellingAmount = sellingAmount
            };

            transaction.Calculate();

            LastTransaction = transaction;
            HasLastTransaction = true;

            Console.WriteLine();
            Console.WriteLine("Transaction saved successfully.");
            PrintCalculation(transaction);
            Console.WriteLine("------------------------------------------------------");
        }

        public static void ViewTransaction()
        {
            if (!HasLastTransaction)
            {
                Console.WriteLine("No transaction available. Please create a new transaction first.");
                return;
            }

            SaleTransaction t = LastTransaction;
            Console.WriteLine("-------------- Last Transaction --------------");
            Console.WriteLine("InvoiceNo: " + t.InvoiceNo);
            Console.WriteLine("Customer: " + t.CustomerName);
            Console.WriteLine("Item: " + t.ItemName);
            Console.WriteLine("Quantity: " + t.Quantity);
            Console.WriteLine("Purchase Amount: " + t.PurchaseAmount.ToString("0.00"));
            Console.WriteLine("Selling Amount: " + t.SellingAmount.ToString("0.00"));
            Console.WriteLine("Status: " + t.ProfitOrLossStatus);
            Console.WriteLine("Profit/Loss Amount: " + t.ProfitOrLossAmount.ToString("0.00"));
            Console.WriteLine("Profit Margin (%): " + t.ProfitMarginPercent.ToString("0.00"));
            Console.WriteLine("--------------------------------------------");
            Console.WriteLine("------------------------------------------------------");
        }

        public static void Recalculate()
        {
            if (!HasLastTransaction)
            {
                Console.WriteLine("No transaction available. Please create a new transaction first.");
                return;
            }

            LastTransaction.Calculate();
            PrintCalculation(LastTransaction);
            Console.WriteLine("------------------------------------------------------");
        }

        private static void PrintCalculation(SaleTransaction t)
        {
            Console.WriteLine("Status: " + t.ProfitOrLossStatus);
            Console.WriteLine("Profit/Loss Amount: " + t.ProfitOrLossAmount.ToString("0.00"));
            Console.WriteLine("Profit Margin (%): " + t.ProfitMarginPercent.ToString("0.00"));
        }
    }

    class Program
    {
        static void Main()
        {
            bool running = true;

            while (running)
            {
                Console.WriteLine("================== QuickMart Traders ==================");
                Console.WriteLine("1. Create New Transaction (Enter Purchase & Selling Details)");
                Console.WriteLine("2. View Last Transaction");
                Console.WriteLine("3. Calculate Profit/Loss (Recompute & Print)");
                Console.WriteLine("4. Exit");
                Console.Write("Enter your option: ");

                string option = Console.ReadLine();
                Console.WriteLine();

                switch (option)
                {
                    case "1":
                        TransactionService.CreateTransaction();
                        break;

                    case "2":
                        TransactionService.ViewTransaction();
                        break;

                    case "3":
                        TransactionService.Recalculate();
                        break;

                    case "4":
                        Console.WriteLine("Thank you. Application closed normally.");
                        running = false;
                        break;

                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }
    }
}
