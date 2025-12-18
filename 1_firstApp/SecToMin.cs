using System;
class SecToMin
{
    public static void ToMin()
    {
        Console.WriteLine("Enter seconds: ");
        int x = Convert.ToInt32(Console.ReadLine());
        Console.WriteLine($"number of minutes = {x/60}");
    }
}