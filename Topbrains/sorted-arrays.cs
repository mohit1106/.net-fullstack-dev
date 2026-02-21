using System;

public class Solution
{
    public T[] MergeSorted<T>(T[] a, T[] b) where T : IComparable<T>
    {
        if (a == null) a = Array.Empty<T>();
        if (b == null) b = Array.Empty<T>();

        T[] result = new T[a.Length + b.Length];

        int i = 0, j = 0, k = 0;

        while (i < a.Length && j < b.Length)
        {
            if (a[i].CompareTo(b[j]) <= 0)
                result[k++] = a[i++];
            else
                result[k++] = b[j++];
        }

        while (i < a.Length)
            result[k++] = a[i++];

        while (j < b.Length)
            result[k++] = b[j++];

        return result;
    }
}
