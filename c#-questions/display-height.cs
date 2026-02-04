using System;

class Program
{
    static string GetHeightCategory(int heightCm)
    {
        if (heightCm < 150) return "Short";
        if (heightCm < 180) return "Average";
        return "Tall";
    }

    static void Main()
    {
        int heightCm = int.Parse(Console.ReadLine());
        Console.WriteLine(GetHeightCategory(heightCm));
    }
}
