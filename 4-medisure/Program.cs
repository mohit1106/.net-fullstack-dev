using System;

namespace MediSureClinicBilling
{
    public class PatientBill
    {
        public string BillId { get; set; }
        public string PatientName { get; set; }
        public bool HasInsurance { get; set; }
        public decimal ConsultationFee { get; set; }
        public decimal LabCharges { get; set; }
        public decimal MedicineCharges { get; set; }
        public decimal GrossAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FinalPayable { get; set; }

        public void Calculate()
        {
            GrossAmount = ConsultationFee + LabCharges + MedicineCharges;
            DiscountAmount = HasInsurance ? GrossAmount * 0.10m : 0m;
            FinalPayable = GrossAmount - DiscountAmount;
        }
    }

    public class BillingService
    {
        public static PatientBill LastBill;
        public static bool HasLastBill;

        public static void CreateBill()
        {
            string billId;
            do
            {
                Console.Write("Enter Bill Id: ");
                billId = Console.ReadLine();
            } while (string.IsNullOrWhiteSpace(billId));

            Console.Write("Enter Patient Name: ");
            string patientName = Console.ReadLine();

            bool hasInsurance;
            while (true)
            {
                Console.Write("Is the patient insured? (Y/N): ");
                string ins = Console.ReadLine();
                if (ins.Equals("Y", StringComparison.OrdinalIgnoreCase))
                {
                    hasInsurance = true;
                    break;
                }
                if (ins.Equals("N", StringComparison.OrdinalIgnoreCase))
                {
                    hasInsurance = false;
                    break;
                }
            }

            decimal consultationFee;
            while (true)
            {
                Console.Write("Enter Consultation Fee: ");
                if (decimal.TryParse(Console.ReadLine(), out consultationFee) && consultationFee > 0)
                    break;
            }

            decimal labCharges;
            while (true)
            {
                Console.Write("Enter Lab Charges: ");
                if (decimal.TryParse(Console.ReadLine(), out labCharges) && labCharges >= 0)
                    break;
            }

            decimal medicineCharges;
            while (true)
            {
                Console.Write("Enter Medicine Charges: ");
                if (decimal.TryParse(Console.ReadLine(), out medicineCharges) && medicineCharges >= 0)
                    break;
            }

            PatientBill bill = new PatientBill
            {
                BillId = billId,
                PatientName = patientName,
                HasInsurance = hasInsurance,
                ConsultationFee = consultationFee,
                LabCharges = labCharges,
                MedicineCharges = medicineCharges
            };

            bill.Calculate();

            LastBill = bill;
            HasLastBill = true;

            Console.WriteLine();
            Console.WriteLine("Bill created successfully.");
            Console.WriteLine("Gross Amount: " + bill.GrossAmount.ToString("0.00"));
            Console.WriteLine("Discount Amount: " + bill.DiscountAmount.ToString("0.00"));
            Console.WriteLine("Final Payable: " + bill.FinalPayable.ToString("0.00"));
            Console.WriteLine("------------------------------------------------------------");
        }

        public static void ViewLastBill()
        {
            if (!HasLastBill)
            {
                Console.WriteLine("No bill available. Please create a new bill first.");
                return;
            }

            PatientBill b = LastBill;
            Console.WriteLine("----------- Last Bill -----------");
            Console.WriteLine("BillId: " + b.BillId);
            Console.WriteLine("Patient: " + b.PatientName);
            Console.WriteLine("Insured: " + (b.HasInsurance ? "Yes" : "No"));
            Console.WriteLine("Consultation Fee: " + b.ConsultationFee.ToString("0.00"));
            Console.WriteLine("Lab Charges: " + b.LabCharges.ToString("0.00"));
            Console.WriteLine("Medicine Charges: " + b.MedicineCharges.ToString("0.00"));
            Console.WriteLine("Gross Amount: " + b.GrossAmount.ToString("0.00"));
            Console.WriteLine("Discount Amount: " + b.DiscountAmount.ToString("0.00"));
            Console.WriteLine("Final Payable: " + b.FinalPayable.ToString("0.00"));
            Console.WriteLine("--------------------------------");
            Console.WriteLine("------------------------------------------------------------");
        }

        public static void ClearLastBill()
        {
            LastBill = null;
            HasLastBill = false;
            Console.WriteLine("Last bill cleared.");
        }
    }

    class Program
    {
        static void Main()
        {
            bool running = true;

            while (running)
            {
                Console.WriteLine("================== MediSure Clinic Billing ==================");
                Console.WriteLine("1. Create New Bill (Enter Patient Details)");
                Console.WriteLine("2. View Last Bill");
                Console.WriteLine("3. Clear Last Bill");
                Console.WriteLine("4. Exit");
                Console.Write("Enter your option: ");

                string input = Console.ReadLine();
                Console.WriteLine();

                switch (input)
                {
                    case "1":
                        BillingService.CreateBill();
                        break;

                    case "2":
                        BillingService.ViewLastBill();
                        break;

                    case "3":
                        BillingService.ClearLastBill();
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
