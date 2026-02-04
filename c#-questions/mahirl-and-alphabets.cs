using System;
using System.Collections.Generic;
using System.Text;

class Program
{
    static bool IsVowel(char c)
    {
        c = char.ToLower(c);
        return c == 'a' || c == 'e' || c == 'i' || c == 'o' || c == 'u';
    }

    static void Main()
    {
        string s1 = Console.ReadLine();
        string s2 = Console.ReadLine();

        HashSet<char> second = new HashSet<char>();
        foreach (char c in s2)
            second.Add(char.ToLower(c));

        StringBuilder filtered = new StringBuilder();

        foreach (char c in s1)
        {
            char lc = char.ToLower(c);
            if (!IsVowel(lc) && second.Contains(lc)) continue;
            filtered.Append(c);
        }

        StringBuilder result = new StringBuilder();
        for (int i = 0; i < filtered.Length; i++)
        {
            if (i == 0 || filtered[i] != filtered[i - 1])
                result.Append(filtered[i]);
        }

        Console.WriteLine(result.ToString());
    }
}

