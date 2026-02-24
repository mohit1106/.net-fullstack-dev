using System;

class Program
{
    static string TimeConversion(int totalSeconds)
    {
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;
        return minutes + ":" + seconds.ToString("D2");
    }

    static void Main()
    {
        int totalSeconds = int.Parse(Console.ReadLine());
        Console.WriteLine(TimeConversion(totalSeconds));
    }
}