using System;
class AOC
{
    public static void area()
    {
        Console.WriteLine("Enter a number");
        double x = Convert.ToDouble(Console.ReadLine());
        Console.WriteLine(Math.PI * x * x);
    }
}