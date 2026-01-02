using System;
using System.Text.RegularExpressions;

class Program{
    public static void Main(){
        // bool res = Regex.IsMatch("abc123", @"\d");
        // bool res = Regex.IsMatch("123_123", @"\d");
        // bool res = Regex.IsMatch("abc", @"\w");

        // Match m = Regex.Match("Amount: 5000", @"\d+");
        Match m = Regex.Match("ASF  852shj@!_", @"\d");
        Console.WriteLine(m.Value);

        MatchCollection matches = Regex.Matches("ASF 52shj@!_", @"\.");
        foreach(Match m1 in matches){
            Console.WriteLine(m1.Value);
        }

        Match m2 = Regex.Match("input.txt", @"\.txt");
        Console.WriteLine(m2.Value);

        Match m3 = Regex.Match(@"\input.txt", @"\\");
        Console.WriteLine(m3.Value);

        Match m4 = Regex.Match(@"\input.txt?Hello", @"lo$");
        Console.WriteLine(m4.Value);

        Match m5 = Regex.Match(@"hello\input.txt?Hello", @"^hello");
        Console.WriteLine(m5.Value);


        string pattern = @"(?<year>\d{4})-(?<month>\d{2})-(?<date>\d{2})";
        string input = "1992-02-23";
        Match m6 = Regex.Match(input, pattern);

        Console.WriteLine(m6.Groups["year"].Value);
        Console.WriteLine(m6.Groups["month"].Value);
        Console.WriteLine(m6.Groups["date"].Value);

        string input1 = "1994-12-12 1993-05-11";
        MatchCollection matches1 = Regex.Matches(input1, pattern);
        foreach (Match m7 in matches1)
        {
            Console.WriteLine(m7.Groups["year"].Value);
        }
        
    }
}