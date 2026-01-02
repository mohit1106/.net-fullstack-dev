using System;
using System.Text.RegularExpressions;

namespace LogProcessing
{
    public class LogParser
    {
        private readonly string validLineRegexPattern =
            @"^\[(TRC|DBG|INF|WRN|ERR|FTL)\]";
        private readonly string splitLineRegexPattern =
            @"<\*{3}>|<={4}>|<\^\*>";
        private readonly string quotedPasswordRegexPattern =
            "\"password\"";
        private readonly string endOfLineRegexPattern =
            @"end-of-line\d+";
        private readonly string weakPasswordRegexPattern =
            @"password[a-zA-Z0-9]+";



        public bool IsValidLine(string text)
        {
            return Regex.IsMatch(text, validLineRegexPattern);
        }



        public string[] SplitLogLine(string text)
        {
            return Regex.Split(text, splitLineRegexPattern);
        }




        public int CountQuotedPasswords(string lines)
        {
            return Regex.Matches(
                lines,
                quotedPasswordRegexPattern,
                RegexOptions.IgnoreCase
            ).Count;
        }




        public string RemoveEndOfLineText(string line)
        {
            return Regex.Replace(line, endOfLineRegexPattern, "");
        }




        public string[] ListLinesWithPasswords(string[] lines)
        {
            string[] result = new string[lines.Length];

            for (int i = 0; i < lines.Length; i++)
            {
                Match match = Regex.Match(
                    lines[i],
                    weakPasswordRegexPattern,
                    RegexOptions.IgnoreCase
                );

                if (match.Success)
                {
                    result[i] = match.Value + ": " + lines[i];
                }
                else
                {
                    result[i] = "--------: " + lines[i];
                }
            }

            return result;
        }

        public static void Main(){
            string[] lines = {"password12ab", "passwer34@$", "password@#ds"};
            LogParser lp = new LogParser();
            string[] res = lp.ListLinesWithPasswords(lines);
            foreach(string str in res){
                Console.WriteLine(str);
            }
        }
    }
}
