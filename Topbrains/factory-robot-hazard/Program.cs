using System;

class RobotSafetyException : Exception
{
    public RobotSafetyException(string message) : base(message) { }
}

class RobotHazardAuditor
{
    public double CalculateHazardRisk(double armPrecision, int workerDensity, string machineryState)
    {
        if (armPrecision < 0.0 || armPrecision > 1.0)
            throw new RobotSafetyException("Error:  Arm precision must be 0.0-1.0");

        if (workerDensity < 1 || workerDensity > 20)
            throw new RobotSafetyException("Error: Worker density must be 1-20");

        double machineRiskFactor;

        if (machineryState == "Worn")
            machineRiskFactor = 1.3;
        else if (machineryState == "Faulty")
            machineRiskFactor = 2.0;
        else if (machineryState == "Critical")
            machineRiskFactor = 3.0;
        else
            throw new RobotSafetyException("Error: Unsupported machinery state");

        return ((1.0 - armPrecision) * 15.0) + (workerDensity * machineRiskFactor);
    }
}

class Program
{
    static void Main()
    {
        try
        {
            Console.WriteLine("Enter Arm Precision (0.0 - 1.0):");
            double armPrecision = double.Parse(Console.ReadLine());

            Console.WriteLine("Enter Worker Density (1 - 20):");
            int workerDensity = int.Parse(Console.ReadLine());

            Console.WriteLine("Enter Machinery State (Worn/Faulty/Critical):");
            string machineryState = Console.ReadLine();

            RobotHazardAuditor auditor = new RobotHazardAuditor();
            double risk = auditor.CalculateHazardRisk(armPrecision, workerDensity, machineryState);

            Console.WriteLine("Robot Hazard Risk Score: " + risk);
        }
        catch (RobotSafetyException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}