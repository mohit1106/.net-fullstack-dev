using System;

class Program
{
    static void SwapRef(ref int a, ref int b)
    {
        a = a + b;
        b = a - b;
        a = a - b;
    }

    static void SwapOut(int a, int b, out int x, out int y)
    {
        x = b;
        y = a;
    }

    static void Main()
    {
        int p = 10, q = 20;

        SwapRef(ref p, ref q);
        Console.WriteLine(p + " " + q);

        int m = 30, n = 40;
        int r, s;

        SwapOut(m, n, out r, out s);
        Console.WriteLine(r + " " + s);
    }
}
