// using System;
// using System.Collections.Generic;

// class Program
// {
//     static void Main()
//     {
//         List<int> divBy2 = new List<int>();
//         List<int> divBy3 = new List<int>();
//         List<int> none = new List<int>();

//         for (int i = 1; i <= 100; i++)
//         {
//             if (i%2 == 0) divBy2.Add(i);
//             if (i%3 == 0) divBy3.Add(i);
//             if (!(i%2 == 0) && !(i%3 == 0)) none.Add(i);
//         }

//         Console.WriteLine("Divisible by 2:");
//         Console.WriteLine(string.Join(", ", divBy2));

//         Console.WriteLine("\nDivisible by 3:");
//         Console.WriteLine(string.Join(", ", divBy3));

//         Console.WriteLine("\nNot divisible by 2 or 3:");
//         Console.WriteLine(string.Join(", ", none));
//     }
// }




public interface ICar
{
    void StartEngine();
    void StopEngine();
    void Accelerate(int amount);
    void Brake(int amount);
}

public class SportsCar : ICar
{
    private int speed = 0;
    private bool engineOn = false;

    public void StartEngine()
    {
        engineOn = true;
        Console.WriteLine("Engine started");
    }

    public void StopEngine()
    {
        engineOn = false;
        speed = 0;
        Console.WriteLine("Engine stopped");
    }

    public void Accelerate(int amount)
    {
        if (!engineOn)
        {
            Console.WriteLine("Start engine first");
            return;
        }

        speed += amount;
        Console.WriteLine($"Accelerating. Speed = {speed}");
    }

    public void Brake(int amount)
    {
        speed = Math.Max(0, speed - amount);
        Console.WriteLine($"Braking. Speed = {speed}");
    }
}


class Program
{
    static void Main()
    {
        ICar car = new SportsCar();

        car.StartEngine();
        car.Accelerate(50);
        car.Brake(20);
        car.StopEngine();
    }
}
