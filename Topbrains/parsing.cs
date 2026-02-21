using System;

public class Solution
{
    public int SumParsedIntegers(string[] tokens)
    {
        int sum = 0;

        if (tokens == null)
            return 0;

        foreach (var token in tokens)
        {
            if (int.TryParse(token, out int value))
            {
                sum += value;
            }
        }

        return sum;
    }
}
