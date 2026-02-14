using System;

namespace LogisticsProShipmentSystem
{
    public class Shipment
    {
        public string ShipmentCode { get; set; }
        public string TransportMode { get; set; }
        public double Weight { get; set; }
        public int StorageDays { get; set; }
    }

    public class ShipmentDetails : Shipment
    {
        public bool ValidateShipmentCode()
        {
            if (ShipmentCode == null)
                return false;
            if (ShipmentCode.Length != 7)
                return false;
            if (!ShipmentCode.StartsWith("GC#"))
                return false;
            string remaining = ShipmentCode.Substring(3);
            foreach (char c in remaining)
            {
                if (!char.IsDigit(c))
                    return false;
            }
            return true;
        }

        public double CalculateTotalCost()
        {
            double ratePerKg = 0;
            if (TransportMode == "Sea")
                ratePerKg = 15.00;
            else if (TransportMode == "Air")
                ratePerKg = 50.00;
            else if (TransportMode == "Land")
                ratePerKg = 25.00;
            else
                ratePerKg = 0;

            double totalCost = (Weight * ratePerKg) + Math.Sqrt(StorageDays);
            return Math.Round(totalCost, 2);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            ShipmentDetails shipment = new ShipmentDetails();

            Console.Write("Enter Shipment Code: ");
            shipment.ShipmentCode = Console.ReadLine();
            if (!shipment.ValidateShipmentCode())
            {
                Console.WriteLine("Invalid shipment code");
                return;
            }

            Console.Write("Enter Transport Mode (Sea/Air/Land): ");
            shipment.TransportMode = Console.ReadLine();

            Console.Write("Enter Weight: ");
            shipment.Weight = Convert.ToDouble(Console.ReadLine());

            Console.Write("Enter Storage Days: ");
            shipment.StorageDays = Convert.ToInt32(Console.ReadLine());

            double cost = shipment.CalculateTotalCost();

            Console.WriteLine($"The total shipping cost is {cost:F2}");
        }
    }
}
