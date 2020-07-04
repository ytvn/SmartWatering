using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SmartWatering.Util
{
    public static class Formater
    {
        public static string Readable(string input)
        {
            string pattern = @"[A-Z][^A-Z]*";
            MatchCollection matches = Regex.Matches(input, pattern);
            string result = "";
            foreach (Match match in matches)
            {
                if (match.Index == 0)
                    result += match.Value;// ToUpperFirstLetter(match.Value);
                else
                {
                    result += " " + match.Value.ToLower();
                }
            }
            return result;
        }
        public static string ToUpperFirstLetter(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;
            // convert to char array of the string
            char[] letters = source.ToCharArray();
            // upper case the first char
            letters[0] = char.ToUpper(letters[0]);
            // return the array made of the new char array
            return new string(letters);
        }
    }
}
