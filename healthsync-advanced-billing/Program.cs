using System;

namespace HealthSyncAdvancedBilling
{
    public abstract class Consultant
    {
        public string ConsultantId { get; set; }

        public bool ValidateConsultantId()
        {
            if (ConsultantId == null)
                return false;

            if (ConsultantId.Length != 6)
                return false;

            if (!ConsultantId.StartsWith("DR"))
                return false;

            string last4 = ConsultantId.Substring(2, 4);

            foreach (char c in last4)
            {
                if (!char.IsDigit(c))
                    return false;
            }

            return true;
        }

        public abstract double CalculateGrossPayout();

        public virtual double CalculateTdsRate(double grossPayout)
        {
            if (grossPayout <= 5000)
                return 0.05; 
            else
                return 0.15; 
        }

        public double CalculateNetPayout()
        {
            double gross = CalculateGrossPayout();
            double tdsRate = CalculateTdsRate(gross);
            double tdsAmount = gross * tdsRate;

            return gross - tdsAmount;
        }
    }

    public class InHouseConsultant : Consultant
    {
        public double MonthlyStipend { get; set; }

        public override double CalculateGrossPayout()
        {
            double allowance = 2000;
            double bonus = 1000;

            return MonthlyStipend + allowance + bonus;
        }
    }

    public class VisitingConsultant : Consultant
    {
        public int ConsultationsCount { get; set; }
        public double RatePerVisit { get; set; }

        public override double CalculateGrossPayout()
        {
            return ConsultationsCount * RatePerVisit;
        }

        public override double CalculateTdsRate(double grossPayout)
        {
            return 0.10;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== HealthSync Advanced Billing System ===");
            Console.Write("Enter Consultant Type (InHouse/Visiting): ");
            string type = Console.ReadLine();

            Console.Write("Enter Doctor ID: ");
            string id = Console.ReadLine();

            Consultant consultant = null;

            if (type == "InHouse")
            {
                InHouseConsultant inHouse = new InHouseConsultant();
                inHouse.ConsultantId = id;

                if (!inHouse.ValidateConsultantId())
                {
                    Console.WriteLine("Invalid doctor id");
                    return;
                }

                Console.Write("Enter Monthly Stipend: ");
                inHouse.MonthlyStipend = Convert.ToDouble(Console.ReadLine());

                consultant = inHouse;
            }
            else if (type == "Visiting")
            {
                VisitingConsultant visiting = new VisitingConsultant();
                visiting.ConsultantId = id;

                if (!visiting.ValidateConsultantId())
                {
                    Console.WriteLine("Invalid doctor id");
                    return;
                }

                Console.Write("Enter Number of Visits: ");
                visiting.ConsultationsCount = Convert.ToInt32(Console.ReadLine());

                Console.Write("Enter Rate Per Visit: ");
                visiting.RatePerVisit = Convert.ToDouble(Console.ReadLine());

                consultant = visiting;
            }
            else
            {
                Console.WriteLine("Invalid consultant type");
                return;
            }

            double gross = consultant.CalculateGrossPayout();
            double tdsRate = consultant.CalculateTdsRate(gross);
            double net = consultant.CalculateNetPayout();

            Console.WriteLine();
            Console.WriteLine($"Gross: {gross:F2} | TDS Applied: {(tdsRate * 100):0}% | Net Payout: {net:F2}");
        }
    }
}