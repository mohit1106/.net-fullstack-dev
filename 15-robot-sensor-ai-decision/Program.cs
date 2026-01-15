using System;
using System.Collections.Generic;
using System.Linq;

namespace AutonomousRobot.AI
{
    public class SensorReading
    {
        public int SensorId { get; set; }
        public string Type { get; set; }
        public double Value { get; set; }
        public DateTime Timestamp { get; set; }
        public double Confidence { get; set; }
    }

    public enum RobotAction
    {
        Stop,
        SlowDown,
        Reroute,
        Continue
    }

    public class DecisionEngine
    {
        public static List<SensorReading> GetRecentReadings(List<SensorReading> sensorHistory, DateTime fromTime)
        {
            return sensorHistory.Where(r => r.Timestamp >= fromTime).ToList();
        }

        public static bool IsBatteryCritical(List<SensorReading> readings)
        {
            return readings.Any(r => r.Type == "Battery" && r.Value < 20);
        }

        public static double GetNearestObstacleDistance(List<SensorReading> readings)
        {
            return readings.Where(r => r.Type == "Distance")
                           .Select(r => r.Value)
                           .DefaultIfEmpty(double.MaxValue)
                           .Min();
        }

        public static bool IsTemperatureSafe(List<SensorReading> readings)
        {
            return readings.Where(r => r.Type == "Temperature")
                           .All(r => r.Value < 90);
        }

        public static double GetAverageVibration(List<SensorReading> readings)
        {
            return readings.Where(r => r.Type == "Vibration")
                           .Select(r => r.Value)
                           .DefaultIfEmpty(0)
                           .Average();
        }

        public static Dictionary<string, double> CalculateSensorHealth(List<SensorReading> sensorHistory)
        {
            return sensorHistory
                   .GroupBy(r => r.Type)
                   .ToDictionary(g => g.Key, g => g.Average(r => r.Confidence));
        }

        public static List<string> DetectFaultySensors(List<SensorReading> sensorHistory)
        {
            return sensorHistory
                   .GroupBy(r => r.Type)
                   .Where(g => g.Count(r => r.Confidence < 0.4) > 2)
                   .Select(g => g.Key)
                   .ToList();
        }

        public static bool IsBatteryDrainingFast(List<SensorReading> sensorHistory)
        {
            var batteryValues = sensorHistory
                .Where(r => r.Type == "Battery")
                .OrderBy(r => r.Timestamp)
                .Select(r => r.Value)
                .ToList();

            return batteryValues.Zip(batteryValues.Skip(1), (a, b) => b < a).All(x => x);
        }

        public static double GetWeightedDistance(List<SensorReading> readings)
        {
            var distanceReadings = readings.Where(r => r.Type == "Distance").ToList();
            var totalConfidence = distanceReadings.Sum(r => r.Confidence);

            return totalConfidence == 0
                ? double.MaxValue
                : distanceReadings.Sum(r => r.Value * r.Confidence) / totalConfidence;
        }

        public static RobotAction DecideRobotAction(List<SensorReading> recentReadings, List<SensorReading> sensorHistory)
        {
            if (IsBatteryCritical(recentReadings))
                return RobotAction.Stop;

            if (GetNearestObstacleDistance(recentReadings) < 1.0)
                return RobotAction.Reroute;

            if (!IsTemperatureSafe(recentReadings) || GetAverageVibration(recentReadings) > 7.5)
                return RobotAction.SlowDown;

            return RobotAction.Continue;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            List<SensorReading> sensorHistory = new List<SensorReading>
            {
                new SensorReading{SensorId=1,Type="Distance",Value=0.8,Confidence=0.9,Timestamp=DateTime.Now.AddSeconds(-5)},
                new SensorReading{SensorId=2,Type="Battery",Value=18,Confidence=0.8,Timestamp=DateTime.Now.AddSeconds(-4)},
                new SensorReading{SensorId=3,Type="Temperature",Value=92,Confidence=0.7,Timestamp=DateTime.Now.AddSeconds(-3)},
                new SensorReading{SensorId=4,Type="Vibration",Value=8.2,Confidence=0.6,Timestamp=DateTime.Now.AddSeconds(-2)},
                new SensorReading{SensorId=5,Type="Battery",Value=75,Confidence=0.9,Timestamp=DateTime.Now.AddSeconds(-1)},
                new SensorReading{SensorId=6,Type="Distance",Value=2.5,Confidence=0.5,Timestamp=DateTime.Now}
            };

            DateTime fromTime = DateTime.Now.AddSeconds(-10);

            List<SensorReading> recentReadings = DecisionEngine.GetRecentReadings(sensorHistory, fromTime);
            bool batteryCritical = DecisionEngine.IsBatteryCritical(recentReadings);
            double nearestDistance = DecisionEngine.GetNearestObstacleDistance(recentReadings);
            bool temperatureSafe = DecisionEngine.IsTemperatureSafe(recentReadings);
            double avgVibration = DecisionEngine.GetAverageVibration(recentReadings);
            Dictionary<string, double> sensorHealth = DecisionEngine.CalculateSensorHealth(sensorHistory);
            List<string> faultySensors = DecisionEngine.DetectFaultySensors(sensorHistory);
            bool batteryDrainingFast = DecisionEngine.IsBatteryDrainingFast(sensorHistory);
            double weightedDistance = DecisionEngine.GetWeightedDistance(recentReadings);
            RobotAction action = DecisionEngine.DecideRobotAction(recentReadings, sensorHistory);

            Console.WriteLine($"Robot Action: {action}");
        }
    }
}
