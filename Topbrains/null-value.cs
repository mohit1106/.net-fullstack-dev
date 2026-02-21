using System;

public class Solution
{
    public double? AverageNonNull(double?[] values)
    {
        if (values == null || values.Length == 0)
            return null;

        double sum = 0;
        int count = 0;

        foreach (var v in values)
        {
            if (v.HasValue)
            {
                sum += v.Value;
                count++;
            }
        }

        if (count == 0)
            return null;

        double avg = sum / count;
        return Math.Round(avg, 2, MidpointRounding.AwayFromZero);
    }
}
