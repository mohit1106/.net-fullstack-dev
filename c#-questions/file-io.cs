using System;
using System.IO;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        var errors = new List<string>();

        foreach (var line in File.ReadAllLines("log.txt"))
        {
            if (line.Contains("ERROR"))
                errors.Add(line);
        }

        File.WriteAllLines("error.txt", errors);
    }
}
