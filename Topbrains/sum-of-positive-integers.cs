using System;

class Program
{
    static int SumPositiveUntilZero(int[] nums)
    {
        int sum = 0;
        foreach (int n in nums)
        {
            if (n == 0)
                break;
            if (n < 0)
                continue;
            sum += n;
        }
        return sum;
    }

    static void Main()
    {
        int n = int.Parse(Console.ReadLine());
        int[] nums = new int[n];
        for (int i = 0; i < n; i++)
            nums[i] = int.Parse(Console.ReadLine());

        Console.WriteLine(SumPositiveUntilZero(nums));
    }
}