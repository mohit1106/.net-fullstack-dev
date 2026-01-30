using System;
using System.Text;
using System.Globalization;

class Solution
{
    static void Main()
    {
        string input = Console.ReadLine();

        StringBuilder sb = new StringBuilder();
        char prev = '\0';

        foreach (char c in input)
        {
            if (c != prev)
                sb.Append(c);
            prev = c;
        }

        string cleaned = sb.ToString().Trim();
        cleaned = System.Text.RegularExpressions.Regex.Replace(cleaned, @"\s+", " ");

        TextInfo ti = CultureInfo.InvariantCulture.TextInfo;
        cleaned = ti.ToTitleCase(cleaned.ToLower());

        Console.WriteLine(cleaned);
    }
}
