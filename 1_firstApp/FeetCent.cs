using System;
class FeetCent
{
    public static void feet()
    {
        Console.WriteLine("Enter Feet");
        double x = Convert.ToDouble(Console.ReadLine());
        Console.WriteLine($"in centimeter = {x * 30.48}");
    }
}